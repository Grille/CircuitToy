using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GGL.IO;

namespace CircuitLib
{
    public static class SaveFile
    {
        public static void Save(string path, Circuit circuit)
        {
            using var bw = new BinaryViewWriter(path);

            var nodes = circuit.Nodes;
            var networks = circuit.Networks;

            bw.CompressAll();

            bw.Write(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                bw.WriteString(node.Name);
                bw.WriteString(node.Description);

                bw.Write(node.Position);
                bw.Write(node.Size);

                writePinArray(node.InputPins);
                writePinArray(node.OutputPins);

                void writePinArray(Pin[] pins)
                {
                    bw.WriteInt32(pins.Length);
                    foreach (var pin in pins)
                    {
                        bw.WriteString(pin.Name);
                        bw.WriteString(pin.Description);
                        bw.WriteBoolean(pin.Active);
                        bw.Write(pin.RelativePosition);
                    }
                }
            }

            bw.Write(networks.Count);
            for (int i = 0; i < networks.Count; i++)
            {
                var network = networks[i];

                bw.WriteString(network.Name);
                bw.WriteString(network.Description);

                bw.WriteInt32(network.GuardPins.Count);
                foreach (var pin in network.GuardPins)
                {
                    bw.Write(pin.Position);
                }

                bw.WriteInt32(network.InputPins.Count);
                foreach (var pin in network.InputPins)
                {
                    bw.WriteInt32(nodes.IndexOf(pin.Owner));
                    bw.WriteInt32(Array.IndexOf(pin.Owner.InputPins, pin));
                }

                bw.WriteInt32(network.OutputPins.Count);
                foreach (var pin in network.OutputPins)
                {
                    bw.WriteInt32(nodes.IndexOf(pin.Owner));
                    bw.WriteInt32(Array.IndexOf(pin.Owner.OutputPins, pin));
                }
            }

            var wires = new List<Wire>();
            foreach (var network in networks)
            {
                foreach (var pin in network.AllPins)
                {
                    foreach (var wire in pin.ConnectedWires)
                    {
                        if (!wires.Contains(wire))
                        {
                            wires.Add(wire);
                        }
                    }
                }
            }

            bw.WriteInt32(wires.Count);
            foreach (var wire in wires)
            {
                writeWirePin(wire.StartPin);
                writeWirePin(wire.EndPin);
            }

            void writeWirePin(Pin pin)
            {
                switch (pin)
                {
                    case NetPin:
                        var nPin = (NetPin)pin;
                        bw.WriteByte(0);
                        bw.WriteInt32(networks.IndexOf(nPin.Owner));
                        bw.WriteInt32(nPin.Owner.GuardPins.IndexOf(nPin));
                        break;
                    case InputPin:
                        var iPin = (InputPin)pin;
                        bw.WriteByte(1);
                        bw.WriteInt32(nodes.IndexOf(iPin.Owner));
                        bw.WriteInt32(Array.IndexOf(iPin.Owner.InputPins, iPin));
                        break;
                    case OutputPin:
                        var oPin = (OutputPin)pin;
                        bw.WriteByte(2);
                        bw.WriteInt32(nodes.IndexOf(oPin.Owner));
                        bw.WriteInt32(Array.IndexOf(oPin.Owner.OutputPins, oPin));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(pin));
                }
            }

        }

        public static Circuit Load(string path)
        {
            var circuit = new Circuit();

            var nodes = circuit.Nodes;
            var networks = circuit.Networks;

            using var br = new BinaryViewReader(path);

            br.DecompressAll();

            int nodeCount = br.ReadInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = circuit.CreateNode<IntegratedCircuit>();

                node.Name = br.ReadString();
                node.Description = br.ReadString();

                node.Position = br.Read<PointF>();
                node.Size = br.Read<SizeF>();

                node.InputPins = readPinArray<InputPin>();
                node.OutputPins = readPinArray<OutputPin>();

                T[] readPinArray<T>() where T : IOPin, new()
                {
                    var pinCount = br.ReadInt32();

                    var pins = new T[pinCount];

                    for (int i = 0; i < pinCount; i++)
                    {
                        var pin = new T();
                        pin.Owner = node;

                        pin.Name = br.ReadString();
                        pin.Description = br.ReadString();
                        pin._active = br.ReadBoolean();
                        pin.RelativePosition = br.Read<PointF>();

                        pins[i] = pin;
                    }

                    return pins;
                }
            }

            int netCount = br.ReadInt32();
            for (int i = 0; i < netCount; i++)
            {
                var net = circuit.CreateNet();

                net.Name = br.ReadString();
                net.Description = br.ReadString();

                int pinCount = br.ReadInt32();
                for (int j = 0; j < pinCount; j++)
                {
                    var pos = br.Read<PointF>();
                    net.CreatePin(pos.X, pos.Y);
                }

                int inPinCount = br.ReadInt32();
                for (int j = 0; j < inPinCount; j++)
                {
                    int index0 = br.ReadInt32();
                    int index1 = br.ReadInt32();
                    net.Add(nodes[index0].InputPins[index1]);
                }

                int outPinCount = br.ReadInt32();
                for (int j = 0; j < outPinCount; j++)
                {
                    int index0 = br.ReadInt32();
                    int index1 = br.ReadInt32();
                    net.Add(nodes[index0].OutputPins[index1]);
                }
            }

            int wireCount = br.ReadInt32();
            for (int i = 0;i < wireCount; i++)
            {
                var pin0 = readWirePin();
                var pin1 = readWirePin();
                pin0.ConnectTo(pin1);
            }

            Pin readWirePin()
            {
                byte type = br.ReadByte();
                int index0 = br.ReadInt32();
                int index1 = br.ReadInt32();

                switch (type)
                {
                    case 0:
                        return networks[index0].GuardPins[index1];
                    case 1:
                        return nodes[index0].InputPins[index1];
                    case 2:
                        return nodes[index0].OutputPins[index1];
                    default:
                        throw new Exception();
                }
            }

            return circuit;


        }
    }
}

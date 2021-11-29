using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Primitives;
using GGL.IO;

namespace CircuitLib
{
    public static class SaveFile
    {
        public static void Save(string path, Circuit circuit)
        {
            using var bw = new BinaryViewWriter(path);
            bw.WriteInt32(0);
            bw.CompressAll();
            WriteCircuit(bw, circuit);
        }

        public static void WriteCircuit(BinaryViewWriter bw, Circuit circuit)
        {
            var nodes = circuit.Nodes;
            var networks = circuit.Networks;

            bw.Write(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                writeNodeType(node);

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

            void writeNodeType(Node node)
            {
                int id = node switch {
                    Input => 0,
                    Output => 1,
                    AndGate => 2,
                    OrGate => 3,
                    XorGate => 4,
                    NotGate => 5,
                    NAndGate => 6,
                    NOrGate => 7,
                    XNorGate => 8,
                    Circuit => 100,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                bw.WriteInt32(id);

                if (id == 100)
                    WriteCircuit(bw, (Circuit)node);
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
            using var br = new BinaryViewReader(path);
            br.ReadInt32();
            br.DecompressAll();
            return ReadCircuit(br);
        }

        public static Circuit ReadCircuit(BinaryViewReader br)
        {
            var circuit = new Circuit();

            var nodes = circuit.Nodes;
            var networks = circuit.Networks;

            int nodeCount = br.ReadInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = readNodeType();

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

                    if (index0 == -1 || index1 == -1)
                        continue;

                    net.Add(nodes[index0].InputPins[index1]);
                }

                int outPinCount = br.ReadInt32();
                for (int j = 0; j < outPinCount; j++)
                {
                    int index0 = br.ReadInt32();
                    int index1 = br.ReadInt32();

                    if (index0 == -1 || index1 == -1)
                        continue;

                    net.Add(nodes[index0].OutputPins[index1]);
                }
            }

            int wireCount = br.ReadInt32();
            for (int i = 0; i < wireCount; i++)
            {
                var pin0 = readWirePin();
                var pin1 = readWirePin();

                if (pin0 == null || pin1 == null)
                    continue;

                pin0.ConnectTo(pin1);
            }

            Node readNodeType()
            {
                int id = br.ReadInt32();

                if (id == 100)
                {
                    var newcirc = ReadCircuit(br);
                    circuit.AddNode(newcirc);
                    return newcirc;
                }

                return id switch {
                    0 => circuit.CreateNode<Input>(),
                    1 => circuit.CreateNode<Output>(),
                    2 => circuit.CreateNode<AndGate>(),
                    3 => circuit.CreateNode<OrGate>(),
                    4 => circuit.CreateNode<XorGate>(),
                    5 => circuit.CreateNode<NotGate>(),
                    6 => circuit.CreateNode<NAndGate>(),
                    7 => circuit.CreateNode<NOrGate>(),
                    8 => circuit.CreateNode<XNorGate>(),
                    100 => circuit.CreateNode<Circuit>(),
                    _ => throw new NotImplementedException(),
                };
            }

            Pin readWirePin()
            {
                byte type = br.ReadByte();
                int index0 = br.ReadInt32();
                int index1 = br.ReadInt32();

                if (index0 == -1 || index1 == -1)
                    return null;

                return type switch {
                    0 => networks[index0].GuardPins[index1],
                    1 => nodes[index0].InputPins[index1],
                    2 => nodes[index0].OutputPins[index1],
                    _ => throw new NotImplementedException(),
                };
            }

            circuit.UpdateIO();
            return circuit;


        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Reflection;
using CircuitLib.Math;
using CircuitLib.Primitives;
using GGL.IO;
using System.IO;
using GGL.IO.Compression;
using System.IO.Compression;

namespace CircuitLib.Serialization;

public class BinarySerializer : IDisposable
{
    private readonly BinaryViewWriter bw;

    public BinarySerializer(string path)
    {
        bw = new BinaryViewWriter(path);
    }

    public BinarySerializer(Stream stream)
    {
        bw = new BinaryViewWriter(stream, true);
    }

    public static void WriteCircuit(Stream stream, Circuit circuit)
    {
        using var bs = new BinarySerializer(stream);
        var types = bs.WriteNodeTypes(circuit);
        bs.WriteCircuit(circuit, types);
    }

    public void WriteHead()
    {
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteString("\0.lcp", LengthPrefix.None, Encoding.ASCII);

        bw.WriteUInt64(0);
        bw.CompressAll(CompressionType.Deflate, CompressionLevel.Optimal);
    }

    public void WriteClipboard(IList<Entity> entities)

    {
        var nodes = new List<Node>();
        var pins = new List<Pin>();
        var netPins = new List<WirePin>();
        var wires = new List<Wire>();

        Vector2 center = Vector2.Zero;

        foreach (var entity in entities)
        {
            if (entity is Node)
            {
                nodes.Add((Node)entity);
                center += entity.Position;
            }

            if (entity is Pin)
            {
                pins.Add((Pin)entity);
            }

            if (entity is WirePin)
            {
                netPins.Add((WirePin)entity);
                center += entity.Position;
            }

            if (entity is Wire)
            {
                wires.Add((Wire)entity);
            }
        }

        foreach (var pin in pins)
        {
            foreach (var wire in pin.ConnectedWires)
            {
                if (!wires.Contains(wire) && pins.Contains(wire.StartPin) && pins.Contains(wire.EndPin)){
                    wires.Add(wire);
                }
            }
        }

        center = (center / (nodes.Count + netPins.Count)).Round();

        var types = WriteNodeTypes(nodes);

        bw.Write(nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
            WriteNode(nodes[i], types, -center);


        bw.WriteInt32(netPins.Count);
        for (int i = 0; i < netPins.Count; i++)
        {
            var pin = netPins[i];
            bw.Write(pin.RelativePosition - center);
        }


        WriteWiresIndices(wires, nodes, netPins);

    }

    public void WriteCircuit(Circuit circuit, List<Type> types)

    {
        var nodes = circuit.Nodes;
        var networks = circuit.Networks;


        WriteNodes(nodes, types);


        bw.Write(networks.Count);
        for (int i = 0; i < networks.Count; i++)
        {
            var network = networks[i];


            WriteNetwork(network, nodes);

        }

    }


    public void WriteNetwork(Network network, IList<Node> nodes)

    {
        WriteLegacyString(network.Name);
        WriteLegacyString(network.Description);

        bw.WriteInt32(network.Pins.NetPins.Count);
        foreach (var pin in network.Pins.NetPins)
        {
            bw.Write(pin.Position);
        }

        bw.WriteInt32(network.Pins.InputPins.Count);
        foreach (var pin in network.Pins.InputPins)
        {
            bw.WriteInt32(nodes.IndexOf(pin.Owner));
            bw.WriteInt32(Array.IndexOf(pin.Owner.InputPins, pin));
        }

        bw.WriteInt32(network.Pins.OutputPins.Count);
        foreach (var pin in network.Pins.OutputPins)
        {
            bw.WriteInt32(nodes.IndexOf(pin.Owner));
            bw.WriteInt32(Array.IndexOf(pin.Owner.OutputPins, pin));
        }

        var wires = new List<Wire>();
        foreach (var pin in network.Pins)
        {
            foreach (var wire in pin.ConnectedWires)
            {
                if (!wires.Contains(wire))
                {
                    wires.Add(wire);
                }
            }
        }


        WriteWiresIndices(wires, nodes, network.Pins.NetPins);

    }

    public void WriteWiresIndices(IList<Wire> wires, IList<Node> nodes, IList<WirePin> netPins)

    {
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
                case WirePin:
                    var nPin = (WirePin)pin;
                    bw.WriteByte(0);
                    bw.WriteInt32(netPins.IndexOf(nPin));
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


    public List<Type> WriteNodeTypes(Node node)
    {
        return WriteNodeTypes(new[] { node });
    }
    public List<Type> WriteNodeTypes(IList<Node> nodes)

    {
        var typeList = new List<Type>();
        var typeNameList = new List<string>();

        BuildTypeListRecursive(nodes, typeList, typeNameList);

        bw.WriteInt32(typeList.Count);
        for (int i = 0; i < typeList.Count; i++)
        {
            WriteLegacyString(typeNameList[i]);
        }

        return typeList;
    }


    public void BuildTypeListRecursive(IList<Node> nodes, IList<Type> types, IList<string> typeNames)

    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var type = node.GetType();
            if (!types.Contains(type))
            {
                types.Add(type);
                typeNames.Add(type.FullName);
            }
            if (node is Circuit)
            {
                var circuit = (Circuit)node;
                BuildTypeListRecursive(circuit.Nodes, types, typeNames);
            }
        }
    }


    public void WriteNodes(IList<Node> nodes, List<Type> types)

    {
        bw.Write(nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];


            WriteNode(node, types, Vector2.Zero);
        }
    }

    public void WriteNode(Node node)
    {
        var types = WriteNodeTypes(node);
        WriteNode(node, types, Vector2.Zero);
    }

    public void WriteNode(Node node, List<Type> types, Vector2 offset)

    {
        int typeID = types.IndexOf(node.GetType());
        if (typeID == -1)
            throw new ArgumentOutOfRangeException();

        bw.WriteInt32(typeID);

        WriteLegacyString(node.Name);
        WriteLegacyString(node.Description);

        bw.Write(node.Position + offset);
        bw.Write(node.Size);

        writePinArray(node.InputPins);
        writePinArray(node.OutputPins);

        if (node is Circuit)
            WriteCircuit((Circuit)node, types);

        void writePinArray(IOPin[] pins)
        {
            bw.WriteInt32(pins.Length);
            foreach (var pin in pins)
            {
                WriteLegacyString(pin.Name);
                WriteLegacyString(pin.Description);
                bw.Write(pin.State);
                bw.Write(pin.RelativePosition);
            }
        }
    }

    private void WriteLegacyString(string str)
    {
        bw.WriteLengthPrefix(LengthPrefix.Default, str.Length);
        bw.WriteString(str, LengthPrefix.None, Encoding.Unicode);
    }

    public void Dispose() => bw.Dispose();
}

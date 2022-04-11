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

namespace CircuitLib.Serialization;

public static class SerializatioUtils
{
    public static void WriteClipboard(BinaryViewWriter bw, IList<Entity> entities)
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

        var types = WriteNodeTypes(bw, nodes);

        bw.Write(nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
            WriteNode(bw, nodes[i], types, -center);


        bw.WriteInt32(netPins.Count);
        for (int i = 0; i < netPins.Count; i++)
        {
            var pin = netPins[i];
            bw.Write(pin.RelativePosition - center);
        }

        WriteWiresIndices(bw, wires, nodes, netPins);

    }

    public static void WriteCircuit(BinaryViewWriter bw, Circuit circuit, List<Type> types)
    {
        var nodes = circuit.Nodes;
        var networks = circuit.Networks;

        WriteNodes(bw, nodes, types);

        bw.Write(networks.Count);
        for (int i = 0; i < networks.Count; i++)
        {
            var network = networks[i];

            WriteNetwork(bw, network, nodes);
        }

    }

    public static void WriteNetwork(BinaryViewWriter bw, Network network, IList<Node> nodes)
    {
        bw.WriteString(network.Name);
        bw.WriteString(network.Description);

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

        WriteWiresIndices(bw, wires, nodes, network.Pins.NetPins);

    }

    public static void WriteWiresIndices(BinaryViewWriter bw, IList<Wire> wires, IList<Node> nodes, IList<WirePin> netPins)
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

    public static List<Type> WriteNodeTypes(BinaryViewWriter bw, Node node)
    {
        return WriteNodeTypes(bw, new[] { node });
    }
    public static List<Type> WriteNodeTypes(BinaryViewWriter bw, IList<Node> nodes)
    {
        var typeList = new List<Type>();
        var typeNameList = new List<string>();

        buildTypeListRecursive(nodes, typeList, typeNameList);

        bw.WriteInt32(typeList.Count);
        for (int i = 0; i < typeList.Count; i++)
        {
            bw.WriteString(typeNameList[i]);
        }

        return typeList;
    }

    public static void buildTypeListRecursive(IList<Node> nodes, IList<Type> types, IList<string> typeNames)
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
                buildTypeListRecursive(circuit.Nodes, types, typeNames);
            }
        }
    }

    public static void WriteNodes(BinaryViewWriter bw, IList<Node> nodes, List<Type> types)
    {
        bw.Write(nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];

            WriteNode(bw, node, types, Vector2.Zero);
        }
    }

    public static void WriteNode(BinaryViewWriter bw, Node node)
    {
        var types = WriteNodeTypes(bw, node);
        WriteNode(bw, node, types, Vector2.Zero);
    }

    public static void WriteNode(BinaryViewWriter bw, Node node, List<Type> types, Vector2 offset)
    {
        int typeID = types.IndexOf(node.GetType());
        if (typeID == -1)
            throw new ArgumentOutOfRangeException();

        bw.WriteInt32(typeID);

        bw.WriteString(node.Name);
        bw.WriteString(node.Description);

        bw.Write(node.Position + offset);
        bw.Write(node.Size);

        writePinArray(node.InputPins);
        writePinArray(node.OutputPins);

        if (node is Circuit)
            WriteCircuit(bw, (Circuit)node, types);

        void writePinArray(IOPin[] pins)
        {
            bw.WriteInt32(pins.Length);
            foreach (var pin in pins)
            {
                bw.WriteString(pin.Name);
                bw.WriteString(pin.Description);
                bw.Write(pin.State);
                bw.Write(pin.RelativePosition);
            }
        }
    }
}

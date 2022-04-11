using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Reflection;
using System.IO;

using CircuitLib.Primitives;
using GGL.IO;

namespace CircuitLib.Serialization;

public static class DeserializationUtils
{

    public static List<Entity> ReadClipboardToCircuit(BinaryViewReader br, Circuit circuit)
    {
        var nodes = new List<Node>();
        var pins = new List<WirePin>();
        var result = new List<Entity>();

        var types = ReadNodeTypes(br);
        int nodeCount = br.ReadInt32();
        for (int i = 0; i < nodeCount; i++)
        {
            var node = ReadNode(br, types);
            circuit.Nodes.Add(node);
            nodes.Add(node);
            result.Add(node);
        }

        var pinCount = br.ReadInt32();
        for (int i = 0; i < pinCount; i++)
        {
            var pos = br.Read<Vector2>();
            var pin = circuit.Networks.Create().Pins.Create(pos);
            pins.Add(pin);
            result.Add(pin);
        }

        ReadWiresIndices(br, nodes, pins);

        return result;
    }

    public static List<Type> ReadNodeTypes(BinaryViewReader br)
    {
        var list = new List<Type>();

        int count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var type = Type.GetType(br.ReadString());
            list.Add(type);
        }

        return list;
    }

    public static void ReadToCircuit(BinaryViewReader br, Circuit circuit, List<Type> types)
    {
        var nodes = circuit.Nodes;
        var networks = circuit.Networks;

        int nodeCount = br.ReadInt32();
        for (int i = 0; i < nodeCount; i++)
        {
            var node = ReadNode(br, types);
            circuit.Nodes.Add(node);
        }

        int netCount = br.ReadInt32();
        for (int i = 0; i < netCount; i++)
        {
            var net = ReadNetwork(br, nodes);
            circuit.Networks.Add(net);
        }

        circuit.UpdateIO();
    }

    public static Network ReadNetwork(BinaryViewReader br, IList<Node> nodes)
    {
        var net = new Network();
        net.BeginEdit();

        net.Name = br.ReadString();
        net.Description = br.ReadString();

        int pinCount = br.ReadInt32();
        for (int j = 0; j < pinCount; j++)
        {
            var pos = br.Read<Vector2>();
            net.Pins.Create(pos);
        }

        int inPinCount = br.ReadInt32();
        for (int j = 0; j < inPinCount; j++)
        {
            int index0 = br.ReadInt32();
            int index1 = br.ReadInt32();

            net.Pins.Add(nodes[index0].InputPins[index1]);
        }

        int outPinCount = br.ReadInt32();
        for (int j = 0; j < outPinCount; j++)
        {
            int index0 = br.ReadInt32();
            int index1 = br.ReadInt32();

            net.Pins.Add(nodes[index0].OutputPins[index1]);
        }

        ReadWiresIndices(br, nodes, net.Pins.NetPins);

        net.EndEdit();

        return net;
    }

    public static void ReadWiresIndices(BinaryViewReader br, IList<Node> nodes, IList<WirePin> netPins)
    {
        int wireCount = br.ReadInt32();
        for (int i = 0; i < wireCount; i++)
        {
            var pin0 = readWirePin();
            var pin1 = readWirePin();

            pin0.ConnectTo(pin1);
        }

        Pin readWirePin()
        {
            byte type = br.ReadByte();

            return type switch {
                0 => netPins[br.ReadInt32()],
                1 => nodes[br.ReadInt32()].InputPins[br.ReadInt32()],
                2 => nodes[br.ReadInt32()].OutputPins[br.ReadInt32()],
                _ => throw new InvalidDataException($"Invalid Wire connection [{type}]"),
            };
        }
    }

    public static Node ReadNode(BinaryViewReader br)
    {
        var types = ReadNodeTypes(br);
        return ReadNode(br, types);
    }

    public static Node ReadNode(BinaryViewReader br, List<Type> types)
    {
        int index = br.ReadInt32();
        var type = types[index];

        var constructor = type.GetConstructor(new Type[] { });
        var node = (Node)constructor.Invoke(null);

        node.Name = br.ReadString();
        node.Description = br.ReadString();

        node.Position = br.Read<Vector2>();
        node.Size = br.Read<Vector2>();

        node.InputPins = readPinArray<InputPin>();
        node.OutputPins = readPinArray<OutputPin>();

        if (type == typeof(Circuit))
            ReadToCircuit(br, (Circuit)node, types);

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
                pin._state = br.Read<State>();
                pin.RelativePosition = br.Read<Vector2>();

                pins[i] = pin;
            }

            return pins;
        }

        return node;
    }
}

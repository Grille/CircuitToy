using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CircuitLib.Math;
using CircuitLib.Primitives;
using GGL;

namespace CircuitLib;

public class Circuit : Node
{
    public List<Node> Nodes;
    public List<Network> Networks;

    private int autoIdCount = 0;

    public Circuit()
    {
        Nodes = new List<Node>();
        Networks = new List<Network>();
    }

    public void AddNode(Node node)
    {
        node.Owner = this;
        Nodes.Add(node);
    }

    public void AddNet(Network net)
    {
        net.Owner = this;
        Networks.Add(net);
    }

    public T CreateNode<T>() where T : Node, new()
    {
        var node = new T();
        AddNode(node);
        node.Name += $"_{autoIdCount++}";
        return node;
    }

    public T CreateNode<T>(float x, float y) where T : Node, new()
    {
        var node = CreateNode<T>();
        node.Position = new PointF(x, y);
        node.RoundPosition();
        return node;
    }
    public Network CreateNet()
    {
        var net = new Network();
        AddNet(net);
        net.Name += $"_{autoIdCount++}";
        return net;
    }

    public void UpdateIO()
    {
        List<InputPin> inputList = new List<InputPin>();
        List<OutputPin> outputList = new List<OutputPin>();

        for (int i = 0;i< Nodes.Count; i++)
        {
            var node = Nodes[i];
            if (node is Input)
            {
                var input = (Input)node;
                var pin = new InputPin(this);
                inputList.Add(pin);

            }
            else if (node is Output)
            {
                var output = (Output)node;
                var pin = new OutputPin(this);
                outputList.Add(pin);
            }
        }

        InputPins = inputList.ToArray();
        OutputPins = outputList.ToArray();

    }

    public override void Update()
    {
        
    }

    public override Entity GetAt(PointF pos)
    {
        foreach (var node in Nodes)
        {
            var obj = node.GetAt(pos);
            if (obj != null)
            {
                return obj;
            }
        }
        foreach (var net in Networks)
        {
            var obj = net.GetAt(pos);
            if (obj != null)
            {
                return obj;
            }
        }
        return null;
    }

    public override void GetFromArea(List<Entity> entities, BoundingBoxF region)
    {
        foreach (var node in Nodes)
        {
            node.GetFromArea(entities, region);
        }
        foreach (var net in Networks)
        {
            net.GetFromArea(entities, region);
        }
    }
}

using System;
using System.Collections.Generic;
using CircuitLib.Primitives;

namespace CircuitLib;

public class Circuit : Node
{
    public List<Node> Nodes;
    public List<Network> Connections;
    public Circuit()
    {
        Nodes = new List<Node>();
        Connections = new List<Network>();
    }

    public void AddNode(Node node)
    {
        node.Owner = this;
        Nodes.Add(node);
    }

    public void AddNet(Network net)
    {
        net.Owner = this;
        Connections.Add(net);
    }

    public T CreateNode<T>() where T : Node, new()
    {
        var node = new T();
        AddNode(node);
        return node;
    }

    public T CreateNode<T>(float x, float y) where T : Node, new()
    {
        var node = CreateNode<T>();
        node.Position = new System.Drawing.PointF(x, y);
        return node;
    }
    public Network CreateNet()
    {
        var net = new Network();
        AddNet(net);
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
}

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

    private List<Input> inputs;
    private List<Output> outputs;

    private int autoIdCount = 0;

    public Circuit()
    {
        Nodes = new List<Node>();
        Networks = new List<Network>();

        inputs = new List<Input>();
        outputs = new List<Output>();
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
        List<InputPin> inputPinList = new List<InputPin>();
        List<OutputPin> outputPinList = new List<OutputPin>();

        inputs.Clear();
        outputs.Clear();

        for (int i = 0;i< Nodes.Count; i++)
        {
            var node = Nodes[i];
            if (node is Input)
            {
                var input = (Input)node;
                inputs.Add(input);
                var pin = new InputPin(this);
                inputPinList.Add(pin);
            }
            else if (node is Output)
            {
                var output = (Output)node;
                outputs.Add(output);
                var pin = new OutputPin(this);
                outputPinList.Add(pin);
            }
        }
        

        InputPins = inputPinList.ToArray();
        OutputPins = outputPinList.ToArray();

    }

    public override void Update()
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            inputs[i]._active = InputPins[i]._active;
        }
        for (int i = 0; i < inputs.Count; i++)
        {
            inputs[i].Update();
        }
        for (int i = 0; i < outputs.Count; i++)
        {
            OutputPins[i].Active = outputs[i]._active;
        }
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

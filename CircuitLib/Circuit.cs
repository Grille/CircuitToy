﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CircuitLib.Math;
using CircuitLib.Primitives;
using System.Numerics;
using GGL;

namespace CircuitLib;

public class Circuit : Node
{
    public List<Node> Nodes;
    public List<Network> Networks;

    private List<Input> inputs;
    private List<Output> outputs;

    private int autoIdCount = 0;

    public bool UseAsync = true;

    public Circuit()
    {
        Nodes = new List<Node>();
        Networks = new List<Network>();

        inputs = new List<Input>();
        outputs = new List<Output>();

        InitPins(new Vector2[0], new Vector2[0]);
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
        return CreateNode<T>(Vector2.Zero);
    }

    public T CreateNode<T>(float x, float y) where T : Node, new()
    {
        return CreateNode<T>(new Vector2(x, y));
    }

    public T CreateNode<T>(float x, float y, string name) where T : Node, new()
    {
        return CreateNode<T>(new Vector2(x, y), name);
    }

    public T CreateNode<T>(Vector2 pos) where T : Node, new()
    {
        return CreateNode<T>(pos, $"AutID_{autoIdCount++}");
    }

    public T CreateNode<T>(Vector2 pos, string name) where T : Node, new()
    {
        var node = new T();
        node.Position = pos;
        node.RoundPosition();
        node.Name = name;

        AddNode(node);
        return node;
    }

    public Network CreateNet()
    {
        var net = new Network();
        AddNet(net);
        net.Name += $"AutID_{autoIdCount++}";
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

        int inCount = InputPins.Length;
        InputStateBuffer = new State[inCount];
        InputNextStateBuffer = new State[inCount];

        int outCount = OutputPins.Length;
        OutputStateBuffer = new State[outCount];

    }

    protected override void OnUpdate()
    {
        PullInputValues();
        for (int i = 0; i < inputs.Count; i++)
        {
            inputs[i].State = InputStateBuffer[i];
        }
        for (int i = 0; i < inputs.Count; i++)
        {
            inputs[i].Update();
        }
    }

    public override void Destroy()
    {
        var refNet = new List<Network>();
        foreach (var net in Networks)
        {
            refNet.Add(net);
        }

        foreach (var net in refNet)
        {
            net.Destroy();
        }

        var refNodes = new List<Node>();
        foreach (var node in Nodes)
        {
            refNodes.Add(node);
        }

        foreach (var node in refNodes)
        {
            node.Destroy();
        }

        base.Destroy();
    }

    public override Entity GetAt(Vector2 pos)
    {
        if (Owner == null)
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
        else
        {
            return base.GetAt(pos);
        }
    }

    public override void GetFromArea(List<Entity> entities, BoundingBox region)
    {
        if (Owner == null)
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
        else
        {
            base.GetFromArea(entities, region);
        }
    }

    public override void Reset(State state = State.Off)
    {
        ForceIdle();

        base.Reset(state);

        foreach (var node in Nodes)
        {
            node.Reset(state);
        }

        foreach (var net in Networks)
        {
            net.Reset(state);
        }
    }

    public override void ForceIdle()
    {
        base.ForceIdle();

        foreach (var node in Nodes)
        {
            node.ForceIdle();
        }

        foreach (var net in Networks)
        {
            net.ForceIdle();
        }
    }

    public override void WaitIdle()
    {
        base.WaitIdle();

        for (int i = 0; i < 100; i++)
        {
            foreach (var node in Nodes)
            {
                node.WaitIdle();
            }

            foreach (var net in Networks)
            {
                net.WaitIdle();
            }
        }
    }


}

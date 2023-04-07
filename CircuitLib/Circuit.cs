using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CircuitLib.Math;
using CircuitLib.Primitives;
using System.Numerics;
using GGL;

namespace CircuitLib;

public partial class Circuit : Node
{
    public NodeList Nodes;
    public NetList Networks;

    private List<Input> inputs;
    private List<Output> outputs;

    public bool UseAsync = true;

    public State DefualtState = State.Low;

    public Circuit()
    {
        Nodes = new NodeList(this);
        Networks = new NetList(this);

        inputs = new List<Input>();
        outputs = new List<Output>();

        InitPins(new Vector2[0], new Vector2[0]);
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

    public void Reset()
    {
        Reset(DefualtState);
    }

    public override void Reset(State state)
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

        foreach (var node in Nodes)
        {
            node.Update();
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
            net.WaitIdle();
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

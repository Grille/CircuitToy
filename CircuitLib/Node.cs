using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;
using GGL.IO;
namespace CircuitLib;

public abstract class Node : AsyncUpdatableEntity
{
    public new Circuit Owner {
        get {
            return (Circuit)base.Owner;
        }
        set {
            base.Owner = value;
        }
    }

    public bool IsFlippedX {
        get; set;
    }
    public bool IsFlippedY {
        get; set;
    }

    public float Rotation {
        get; set;
    }

    public string DisplayName;

    public InputPin[] InputPins;
    public OutputPin[] OutputPins;

    internal protected State[] InputNextStateBuffer;
    internal protected State[] InputStateBuffer;
    internal protected State[] OutputStateBuffer;

    public BoundingBox ChipBounds;

    private Vector2 _size;

    int statsOutSignalSendCount = 0;
    int statsOutSignalDiscardedCout = 0;

    public Vector2 Size {
        get { return _size; }
        set {
            _size = value;
            CalcBoundings();
        }
    }

    private Vector2 _pos;
    public override Vector2 Position {
        get { return _pos; }
        set { 
            _pos = value;
            if (InputPins != null)
            {
                foreach (var pin in InputPins)
                {
                    pin.UpdatePosition();
                }
            }
            if (OutputPins != null)
            {
                foreach (var pin in OutputPins)
                {
                    pin.UpdatePosition();
                }
            }
            CalcBoundings();
        }
    }

    public void SendOutputSignal(int outid)
    {
        if (OutputPins[outid].State != OutputStateBuffer[outid])
        {
            OutputPins[outid].State = OutputStateBuffer[outid];
            OutputPins[outid].ConnectedNetwork?.Update();
            statsOutSignalSendCount++;
        }
        else
        {
            statsOutSignalDiscardedCout++;
        }
    }
    public void SendOutputSignal()
    {
        for (int i = 0; i < OutputPins.Length; i++)
        {
            SendOutputSignal(i);
        }
    }

    protected void PullInputValues()
    {
        for (int i = 0; i < InputPins.Length; i++)
            InputStateBuffer[i] = InputPins[i].State;
    }

    public override void Destroy()
    {
        if (InputPins != null)
        {
            foreach (var pin in InputPins)
            {
                pin.Destroy();
            }
        }
        if (OutputPins != null)
        {
            foreach (var pin in OutputPins)
            {
                pin.Destroy();
            }
        }
        Owner?.Nodes.Remove(this);
        base.Destroy();
    }

    public void ConnectTo(Node target, int outId, int inId)
    {
        var outPin = OutputPins[outId];
        var inPin = target.InputPins[inId];

        if (outPin.ConnectedNetwork != null)
        {
            outPin.ConnectedNetwork.ConnectFromTo(outPin, inPin);
        }
        else if (inPin.ConnectedNetwork != null)
        {
            inPin.ConnectedNetwork.ConnectFromTo(inPin, outPin);
        }
        else
        {
            var net = Owner.Networks.Create();
            net.Pins.Add(outPin);
            net.ConnectFromTo(outPin, inPin);
        }
    }

    public override void CalcBoundings()
    {
        var bounds = new BoundingBox(_pos - _size / 2, _pos + _size / 2, 0.1f);

        ChipBounds = bounds;

        if (InputPins != null)
        {
            foreach (var pin in InputPins)
            {
                bounds.ExtendWith(pin.Bounds);
            }
        }
        if (OutputPins != null)
        {
            foreach (var pin in OutputPins)
            {
                bounds.ExtendWith(pin.Bounds);
            }
        }

        Bounds = bounds;
    }

    public override Entity GetAt(Vector2 pos)
    {
        if (!Bounds.IsInside(pos))
            return null;

        if (Bounds.IsInside(pos))
        {
            foreach (var pin in OutputPins)
            {
                var pinref = pin.GetAt(pos);
                if (pinref != null)
                {
                    return pinref;
                }
            }
            foreach (var pin in InputPins)
            {
                var pinref = pin.GetAt(pos);
                if (pinref != null)
                {
                    return pinref;
                }
            }
            if (ChipBounds.IsInside(pos))
                return this;
        }
        return null;
    }

    public override void GetFromArea(List<Entity> entities, BoundingBox region)
    {
        if (!Bounds.IsColliding(region))
            return;

        if (Bounds.IsColliding(region))
        {
            foreach (var pin in OutputPins)
            {
                pin.GetFromArea(entities, region);
            }
            foreach (var pin in InputPins)
            {
                pin.GetFromArea(entities, region);
            }
            if (ChipBounds.IsColliding(region))
                entities.Add(this);
        }
    }

    public void FlipX()
    {
        foreach (var pin in OutputPins)
            pin.RelativePosition = new Vector2(-pin.RelativePosition.X, pin.RelativePosition.Y);

        foreach (var pin in InputPins)
            pin.RelativePosition = new Vector2(-pin.RelativePosition.X, pin.RelativePosition.Y);
    }

    public void FlipY()
    {
        foreach (var pin in OutputPins)
            pin.RelativePosition = new Vector2(pin.RelativePosition.X, -pin.RelativePosition.Y);

        foreach (var pin in InputPins)
            pin.RelativePosition = new Vector2(pin.RelativePosition.X, -pin.RelativePosition.Y);
    }

    public void RotateAdd90Deg()
    {
        Size = new Vector2(Size.Y, Size.X);

        foreach (var pin in OutputPins)
            pin.RelativePosition = new Vector2(pin.RelativePosition.Y, pin.RelativePosition.X);

        foreach (var pin in InputPins)
            pin.RelativePosition = new Vector2(pin.RelativePosition.Y, pin.RelativePosition.X);
    }

    public void RotateSub90Deg()
    {
        Size = new Vector2(Size.Y, Size.X);

        foreach (var pin in OutputPins)
            pin.RelativePosition = new Vector2(-pin.RelativePosition.Y, -pin.RelativePosition.X);

        foreach (var pin in InputPins)
            pin.RelativePosition = new Vector2(-pin.RelativePosition.Y, -pin.RelativePosition.X);
    }

    public void Rotate180Deg()
    {
        foreach (var pin in OutputPins)
            pin.RelativePosition = new Vector2(-pin.RelativePosition.X, -pin.RelativePosition.Y);

        foreach (var pin in InputPins)
            pin.RelativePosition = new Vector2(-pin.RelativePosition.X, -pin.RelativePosition.Y);
    }

    public virtual void Reset(State state = State.Off)
    {
        ForceIdle();

        if (InputPins != null)
        foreach (var pin in InputPins)
        {
            pin.State = state;
        }

        if (OutputPins != null)
        foreach (var pin in OutputPins)
        {
            pin.State = state;
        }
    }

    protected void InitPins(Vector2[] inputs, Vector2[] outputs)
    {
        int inCount = inputs.Length;
        int outCount = outputs.Length;

        InputPins = new InputPin[inCount];
        OutputPins = new OutputPin[outCount];

        InputStateBuffer = new State[inCount];
        InputNextStateBuffer = new State[inCount];
        OutputStateBuffer = new State[outCount];

        for (int i = 0; i < inCount; i++)
        {
            InputPins[i] = new InputPin(this, inputs[i]);
        }

        for (int i = 0; i < outCount; i++)
        {
            OutputPins[i] = new OutputPin(this, outputs[i]);
        }


        CalcBoundings();
    }

    protected virtual void OnRender()
    {
    }

    protected virtual void OnSave(BinaryViewWriter bw)
    {
    }

    protected virtual void OnLoad(BinaryViewReader br)
    {
    }

    public override string GetDebugStr()
    {
        var sb = new StringBuilder();


        sb.AppendLine($"Node::{GetType().Name} ID[{ID}] N:{Name} D:{DisplayName}");
        sb.AppendLine($"Pos: {Position}");
        sb.AppendLine($"Task.Exists: {UpdateTask != null}");
        sb.AppendLine($"Semaphores:");
        sb.AppendLine($" - UpdateState: {UpdateState}");
        sb.AppendLine($"Stats:");
        sb.AppendLine($" - UpdateCount: {StatsUpdatesCount}");
        sb.AppendLine($"    - Discarded: {StatsUpdatesDiscardCount}");
        sb.AppendLine($"    - Queued:    {StatsUpdatesQueuedCount}");
        sb.AppendLine($"    - Run:       {StatsUpdatesRunCount}");
        sb.AppendLine($" - OutSignalCount: {statsOutSignalSendCount + statsOutSignalDiscardedCout}");
        sb.AppendLine($"    - Discarded: {statsOutSignalDiscardedCout}");
        sb.AppendLine($"    - Send:      {statsOutSignalSendCount}");
        sb.AppendLine($"");

        return sb.ToString();
    }

}


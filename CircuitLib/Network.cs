using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;

namespace CircuitLib;

public partial class Network : AsyncUpdatableEntity
{
    public new Circuit Owner {
        get {
            return (Circuit)base.Owner;
        }
        set {
            base.Owner = value;
        }
    }

    public PinList Pins;
    public WireList Wires;

    internal protected State[] InputStateBuffer;
    private int editScope = 0;

    public override Vector2 Position {
        get { return Vector2.Zero; }
        set {  }
    }

    private State _active = State.Off;
    public State State {
        get => _active;
        set => _active = value;
    }

    public Network()
    {
        Pins = new PinList(this);
        Wires = new WireList(this);
    }

    public void ConnectFromTo(Pin pin0, Pin pin1)
    {
        BeginEdit();

        if (pin0 == pin1)
            return;

        if (pin1 is IOPin)
        {
            var pin = (IOPin)pin1;
            if (pin.ConnectedNetwork == null)
                Pins.Add(pin);
            else if (pin.ConnectedNetwork != this)
                Join(pin.ConnectedNetwork);
        }
        else if (pin1 is WirePin)
        {
            var pin = (WirePin)pin1;
            if (pin.Owner == null)
                Pins.Add(pin);
            else if (pin.Owner != this)
                Join(pin.Owner);
        }
        Wires.Add(new Wire(this, pin0, pin1));

        EndEdit();
    }
    public void ConnectFromTo(Pin pin0, Vector2 pos1)
    {
        BeginEdit();
        var entity = Owner.GetAt(pos1);
        if (entity == null)
        {
            var rpos1 = pos1.Round();
            var pin1 = Pins.Create();
            pin1.Position = rpos1;
            Wires.Add(new Wire(this, pin0, pin1));
        }
        else if (entity is Pin)
        {
            ConnectFromTo(pin0, (Pin)entity);
        }

        EndEdit();
    }
    public void Disconnect(Pin pin0, Pin pin1)
    {
        BeginEdit();

        bool found = false;
        for (int i = 0;i<pin0.ConnectedWires.Count;i++)
        {
            var wire = pin0.ConnectedWires[i];
            if (wire.StartPin == pin1 || wire.EndPin == pin1)
            {
                wire.Destroy();
                found = true;
            }
        }
        if (!found)
            throw new InvalidOperationException("Pin not connectet");

        EndEdit();
    }

    public void Join(Network network)
    {
        BeginEdit();
        network.BeginEdit();

        if (network == this)
            throw new InvalidOperationException("Is same Network");

        var refPins = new List<Pin>();
        foreach (var pin in network.Pins)
        {
            refPins.Add(pin);
        }

        foreach (var pin in refPins)
        {
            network.Pins.Remove(pin);
            Pins.Add(pin);
        }

        var refWires = new List<Wire>();
        foreach (var wire in network.Wires)
        {
            refWires.Add(wire);
        }

        foreach (var wire in refWires)
        {
            network.Wires.Remove(wire);
            Wires.Add(wire);
            wire.Owner = this;
        }

        EndEdit();
        network.EndEdit();

        network.Destroy();
    }

    private void split()
    {
        if (Pins.Count == 0)
        {
            Destroy();
            return;
        }

        editScope++;

        var connectetPins = Pins[0].GetConnectedPins();
        var disconnectetPins = new List<Pin>();

        var refPins = new List<Pin>();
        foreach (var pin in Pins)
        {
            refPins.Add(pin);
        }

        foreach (var pin in refPins)
        {
            if (!connectetPins.Contains(pin))
            {
                disconnectetPins.Add(pin);
                Pins.Remove(pin);
            }
        }

        if (disconnectetPins.Count > 0)
        {
            var net = Owner.Networks.Create();
            foreach (var pin in disconnectetPins)
            {
                net.Pins.Add(pin);
            }
            net.overtakeWires();
            net.split();
        }
        editScope--;
    }

    public override void Destroy()
    {
        editScope++;

        var refPins = new List<Pin>();
        foreach (var pin in Pins)
        {
            refPins.Add(pin);
        }

        foreach (var pin in refPins)
        {
            Pins.Remove(pin);
        }

        var refWires = new List<Wire>();
        foreach (var wire in Wires)
        {
            refWires.Add(wire);
        }

        foreach (var wire in refWires)
        {
            Wires.Remove(wire);
        }

        editScope--;

        Owner.Networks.Remove(this);
        base.Destroy();
    }

    private void overtakeWires()
    {
        foreach (Pin pin in Pins)
        {
            foreach (Wire wire in pin.ConnectedWires)
            {
                if (wire.Owner != this)
                {
                    wire.Owner.Wires.Remove(wire);
                    Wires.Add(wire);
                }
            }
        }
    }

    protected void PullInputValues()
    {
        
    }
    protected override void OnUpdate()
    {
        var oldState = State;
        int inCount = Pins.InputPins.Count;
        int outCount = Pins.OutputPins.Count;

        if (outCount == 1)
        {
            State = Pins.OutputPins[0].State;
        }
        else { 
            State = Owner.DefualtState;

            int offCount = 0;
            int lowCount = 0;
            int highCount = 0;
            int errorCount = 0; 

            for (int i = 0; i < outCount; i++)
            {
                switch (Pins.OutputPins[i].State)
                {
                    case State.Off: offCount++; break; 
                    case State.Low: lowCount++; break;
                    case State.High: highCount++; break;
                    case State.Error: errorCount++; break;
                }
            }

            if (lowCount + highCount > 1)
                State = State.Error;
            else if (lowCount == 1)
                State = State.Low;
            else if (highCount == 1)
                State = State.High;
            else if (errorCount > 0)
                State = State.Error;
            else if (offCount > 0)
                State = State.Off;
        }

        //if (State == oldState)
        //    return;

        for (int i = 0; i < inCount; i++)
        {
            var pin = Pins.InputPins[i];
            pin.State = State;
        }

        //if (State == State.Error)
        //    return;

        for (int i = 0; i < inCount; i++)
        {
            var pin = Pins.InputPins[i];
            pin.Owner.Update();
        }
    }

    public override void CalcBoundings()
    {
        if (Pins.Count > 0)
        {
            Bounds = Pins[0].Bounds;
            for (int i = 1; i < Pins.Count; i++)
            {
                Bounds.ExtendWith(Pins[i].Bounds);
            }
        }
    }

    public override Entity GetAt(Vector2 pos)
    {
        if (!Bounds.IsInside(pos))
            return null;

        foreach (var pin in Pins)
        {
            var obj = pin.GetAt(pos);
            if (obj != null)
            {
                return obj;
            }
        }
        foreach (var wire in Wires)
        {
            var obj = wire.GetAt(pos);
            if (obj != null)
            {
                return obj;
            }
        }
        return null;
    }

    public override void GetFromArea(List<Entity> entities, BoundingBox region)
    {
        if (!Bounds.IsColliding(region))
            return;

        foreach (var pin in Pins)
        {
            pin.GetFromArea(entities, region);
        }
        foreach (var wire in Wires)
        {
            wire.GetFromArea(entities, region);
        }
    }

    public void Cleanup()
    {
        if (editScope > 0)
            return;

        split();
        CalcBoundings();
        Update();
    }

    public void BeginEdit()
    {
        editScope++;
        WaitIdle();
    }

    public void EndEdit()
    {
        editScope--;
        Cleanup();
        
    }

    public void Reset(State state = State.Off)
    {
        State = state;
    }

    public override string GetDebugStr()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Network::{GetType().Name} ID[{ID}] N:{Name}");
        sb.AppendLine($"Task.Exists: {UpdateTask != null}");
        sb.AppendLine($"Semaphores:");
        sb.AppendLine($" - UpdateState: {UpdateState}");
        sb.AppendLine($"Stats:");
        sb.AppendLine($" - UpdateCount: {StatsUpdatesCount}");
        sb.AppendLine($"    - Discarded: {StatsUpdatesDiscardCount}");
        sb.AppendLine($"    - Queued:    {StatsUpdatesQueuedCount}");
        sb.AppendLine($"    - Run:       {StatsUpdatesRunCount}");

        return sb.ToString();
    }
}


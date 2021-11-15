using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;

namespace CircuitLib;

public class Network : Entity
{
    public new Circuit Owner {
        get {
            return (Circuit)base.Owner;
        }
        set {
            base.Owner = value;
        }
    }

    public List<InputPin> InputPins = new List<InputPin>();
    public List<OutputPin> OutputPins = new List<OutputPin>();
    public List<NetPin> GuardPins = new List<NetPin>();
    public List<Pin> AllPins = new List<Pin>();
    public List<Wire> Wires = new List<Wire>();

    private bool enableSpilt = true;
    public override PointF Position {
        get { return Owner.Position; }
        set { Owner.Position = value; }
    }

    private bool _active = false;
    public override bool Active {
        get => _active;
        set => _active = value;
    }

    public Network()
    {
    }

    public Network(List<Pin> pins)
    {

    }

    public void ConnectFromTo(Pin pin0, Pin pin1)
    {
        if (pin0 == pin1)
            return;

        if (pin1 is IOPin)
        {
            var pin = (IOPin)pin1;
            if (pin.ConnectedNetwork == null)
                Add(pin);
            else if (pin.ConnectedNetwork != this)
                Join(pin.ConnectedNetwork);
        }
        else if (pin1 is NetPin)
        {
            var pin = (NetPin)pin1;
            if (pin.Owner == null)
                Add(pin);
            else if (pin.Owner != this)
                Join(pin.Owner);
        }
        Wires.Add(new Wire(this, pin0, pin1));
    }
    public void ConnectFromTo(Pin pin0, PointF pos1)
    {
        var entity = Owner.GetAt(pos1);
        if (entity == null)
        {
            var rpos1 = new PointF(MathF.Round(pos1.X), MathF.Round(pos1.Y));
            var pin1 = CreatePin();
            pin1.Position = rpos1;
            Wires.Add(new Wire(this, pin0, pin1));
        }
        else if (entity is Pin)
        {
            ConnectFromTo(pin0, (Pin)entity);
        }
    }
    public void Disconnect(Pin pin0, Pin pin1)
    {
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

        //Split();
    }

    public NetPin CreatePin(float x, float y)
    {
        var pin = new NetPin(this, x, y);
        GuardPins.Add(pin);
        AllPins.Add(pin);
        return pin;
    }
    public NetPin CreatePin()
    {
        var pin = new NetPin(this);
        GuardPins.Add(pin);
        AllPins.Add(pin);
        return pin;
    }
    //public static Network Ground = new Network();
    public void Add(Pin pin)
    {
        if (pin is InputPin)
        {
            var inPin = (InputPin)pin;
            if (inPin.ConnectedNetwork != null)
                throw new InvalidOperationException($"InputPin already in other Network!");
            if (InputPins.Contains(inPin))
                throw new InvalidOperationException($"InputPin already in this Network!");
            inPin.ConnectedNetwork = this;
            InputPins.Add(inPin);
        }
        else if (pin is OutputPin) {
            var outPin = (OutputPin)pin;
            if (outPin.ConnectedNetwork != null)
                throw new InvalidOperationException($"OutputPin already in other Network!");
            if (OutputPins.Contains(outPin))
                throw new InvalidOperationException($"OutputPin already in this Network!");
            outPin.ConnectedNetwork = this;
            OutputPins.Add(outPin);
        }
        else if (pin is NetPin)
        {
            var netPin = (NetPin)pin;
            if (netPin.Owner != null)
                throw new InvalidOperationException($"Pin already has Owner!");
            if (GuardPins.Contains(netPin))
                throw new InvalidOperationException($"Pin already in this Network!");
            netPin.Owner = this;
            GuardPins.Add(netPin);
        }
        else
        {
            throw new ArgumentException($"Invalid Pin!");
        }
        AllPins.Add(pin);
        CalcBoundings();
    }

    public void Join(Network network)
    {
        network.enableSpilt = false;

        if (network == this)
            throw new InvalidOperationException("Is same Network");

        var refPins = new List<Pin>();
        foreach (var pin in network.AllPins)
        {
            refPins.Add(pin);
        }

        foreach (var pin in refPins)
        {
            network.removeFromList(pin);
            Add(pin);
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



        network.Destroy();
        CalcBoundings();
    }

    private void split()
    {
        enableSpilt = false;

        if (AllPins.Count == 0)
        {
            Destroy();
            return;
        }

        var connectetPins = AllPins[0].GetConnectedPins();
        var disconnectetPins = new List<Pin>();

        var refPins = new List<Pin>();
        foreach (var pin in AllPins)
        {
            refPins.Add(pin);
        }

        foreach (var pin in refPins)
        {
            if (!connectetPins.Contains(pin))
            {
                disconnectetPins.Add(pin);
                removeFromList(pin);
            }
        }

        if (disconnectetPins.Count > 0)
        {
            var net = Owner.CreateNet();
            foreach (var pin in disconnectetPins)
            {
                net.Add(pin);
            }
            net.overtakeWires();
            net.split();
        }

        enableSpilt = true;
    }

    public override void Destroy()
    {
        enableSpilt = false;

        var refPins = new List<Pin>();
        foreach (var pin in AllPins)
        {
            refPins.Add(pin);
        }

        foreach (var pin in refPins)
        {
            removeFromList(pin);
        }

        var refWires = new List<Wire>();
        foreach (var wire in Wires)
        {
            refWires.Add(wire);
        }

        foreach (var wire in refWires)
        {
            Wires.Remove(wire);
            wire.Owner = this;
        }

        Owner.Networks.Remove(this);
        base.Destroy();
    }


    public void Remove(Pin pin)
    {
        removeFromList(pin);
        pin.DestroyConnections();
        if (enableSpilt)
            split();
        CalcBoundings();
    }

    public void Remove(Wire wire)
    {
        removeFromList(wire);
        if (enableSpilt)
            split();
        CalcBoundings();
    }

    private void removeFromList(Pin pin)
    {
        if (pin is InputPin)
        {
            var inPin = (InputPin)pin;
            if (!InputPins.Contains(inPin))
                throw new InvalidOperationException($"InputPin not in this Network!");
            inPin.ConnectedNetwork = null;
            InputPins.Remove(inPin);
        }
        else if (pin is OutputPin)
        {
            var outPin = (OutputPin)pin;
            if (!OutputPins.Contains(outPin))
                throw new InvalidOperationException($"OutputPin not in this Network!");
            outPin.ConnectedNetwork = null;
            OutputPins.Remove(outPin);
        }
        else if (pin is NetPin)
        {
            var netPin = (NetPin)pin;
            if (!GuardPins.Contains(netPin))
                throw new InvalidOperationException($"Pin not in this Network!");
            netPin.Owner = null;
            GuardPins.Remove(netPin);
        }
        else
        {
            throw new ArgumentException($"Invalid Pin!");
        }
        AllPins.Remove(pin);
        CalcBoundings();
    }

    private void removeFromList(Wire wire)
    {
        Wires.Remove(wire);
        wire.Owner = null;
    }

    private void overtakeWires()
    {
        foreach (Pin pin in AllPins)
        {
            foreach (Wire wire in pin.ConnectedWires)
            {
                if (wire.Owner != this)
                {
                    wire.Owner.removeFromList(wire);
                    Wires.Add(wire);
                    wire.Owner = this;
                }
            }
        }
    }


    public void Update()
    {
        Active = false;

        for (int i = 0; i < OutputPins.Count; i++)
        {
            if (OutputPins[i].Active)
            {
                Active = true;
            }
        }

        foreach (var pin in InputPins)
        {
            pin.Active = Active;
        }
        foreach (var pin in GuardPins)
        {
            pin.Active = Active;
        }
        foreach (var wire in Wires)
        {
            wire.Active = Active;
        }
    }

    public override void CalcBoundings()
    {
        if (AllPins.Count > 0)
        {
            Bounds = AllPins[0].Bounds;
            for (int i = 1; i < AllPins.Count; i++)
            {
                Bounds.ExtendWith(AllPins[i].Bounds);
            }
        }
    }

    public override Entity GetAt(PointF pos)
    {
        foreach (var pin in AllPins)
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

    public override void GetFromArea(List<Entity> entities, BoundingBoxF region)
    {
        foreach (var pin in AllPins)
        {
            pin.GetFromArea(entities, region);
        }
        foreach (var wire in Wires)
        {
            wire.GetFromArea(entities, region);
        }
    }
}


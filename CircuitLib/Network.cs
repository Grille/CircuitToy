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

    public override PointF Position {
        get { return Owner.Position; }
        set { Owner.Position = value; }
    }

    private bool _active = false;
    public override bool Active {
        get => _active;
        set => _active = value;
    }

    public void ConnectFromTo(Pin pin0, Pin pin1)
    {
        Add(pin1);
        Wires.Add(new Wire(this, pin0, pin1));
    }
    public void ConnectFromTo(Pin pin0, PointF pos1)
    {
        var rpos1 = new PointF(MathF.Round(pos1.X), MathF.Round(pos1.Y));
        var pin1 = CreatePin();
        pin1.Position = rpos1;
        Wires.Add(new Wire(this, pin0, pin1));
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
        foreach (InputPin pin in network.InputPins)
        {
            pin.ConnectedNetwork = null;
        }
        foreach (OutputPin pin in network.OutputPins)
        {
            pin.ConnectedNetwork = null;
        }
        foreach (Pin pin in network.GuardPins)
        {
            pin.Destroy();
        }
        CalcBoundings();
    }

    public override void Destroy()
    {
        foreach (InputPin pin in InputPins)
        {
            Remove(pin);
        }
        foreach (OutputPin pin in OutputPins)
        {
            Remove(pin);
        }
        foreach (Pin pin in GuardPins)
        {
            pin.Destroy();
        }
    }


    public void Remove(Pin pin)
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


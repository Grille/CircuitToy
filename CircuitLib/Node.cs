using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;

namespace CircuitLib;

public abstract class Node : Entity
{
    public new Circuit Owner {
        get {
            return (Circuit)base.Owner;
        }
        set {
            base.Owner = value;
        }
    }

    public string DisplayName;

    public InputPin[] InputPins;
    public OutputPin[] OutputPins;

    public BoundingBox ChipBounds;

    private Vector2 _size;
    public Vector2 Size {
        get { return _size; }
        set { 
            _size = value;
            CalcBoundings();
        }
    }

    internal bool _active = false;
    public override bool Active {
        get => _active;
        set => _active = value;
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

    public abstract void Update();

    public override void Destroy()
    {
        foreach (var pin in InputPins)
        {
            pin.Destroy();
        }
        foreach (var pin in OutputPins)
        {
            pin.Destroy();
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
            var net = Owner.CreateNet();
            net.Add(outPin);
            net.ConnectFromTo(outPin, inPin);
        }
    }

    public override void CalcBoundings()
    {
        var bounds = new BoundingBox(
            _pos.X - _size.X / 2 -0.1f,
            _pos.Y - _size.Y / 2 -0.1f,
            _pos.X + _size.X / 2 + 0.1f,
            _pos.Y + _size.Y / 2 + 0.1f
        );

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
}


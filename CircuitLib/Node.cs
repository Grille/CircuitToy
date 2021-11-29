using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
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

    public BoundingBoxF ChipBounds;

    private SizeF _size;
    public SizeF Size {
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

    private PointF _pos;
    public override PointF Position {
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
        Network net;
        if (target.InputPins[inId].ConnectedNetwork == null)
        {
            net = Owner.CreateNet();
            net.Add(OutputPins[outId]);
        }
        else
        {
            net = target.InputPins[inId].ConnectedNetwork;
        }

        net.ConnectFromTo(OutputPins[outId], target.InputPins[inId]);
    }

    public override void CalcBoundings()
    {
        var bounds = new BoundingBoxF(
            _pos.X - _size.Width / 2 -0.1f,
            _pos.X + _size.Width / 2 +0.1f,
            _pos.Y - _size.Height / 2 -0.1f,
            _pos.Y + _size.Height / 2 + 0.1f
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

    public override Entity GetAt(PointF pos)
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

    public override void GetFromArea(List<Entity> entities, BoundingBoxF region)
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


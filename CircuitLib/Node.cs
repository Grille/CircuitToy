using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;

namespace CircuitLib;

public abstract class Node : WorldObj
{
    public Circuit Owner;

    public InputPin[] InputPins;
    public OutputPin[] OutputPins;

    public int ID = -1;
    public string Name = "";
    public string Description = "";
    public bool Active = false;

    internal BoundingBoxF ChipBounds;

    private SizeF _size;
    public SizeF Size {
        get { return _size; }
        set { 
            _size = value;
            CalcBoundings();
        }
    }

    private PointF _pos;
    public PointF Position {
        get { return _pos; }
        set { 
            _pos = value;
            foreach (var pin in InputPins)
            {
                pin.UpdatePosition();
            }
            foreach (var pin in OutputPins)
            {
                pin.UpdatePosition();
            }
            CalcBoundings();
        }
    }

    public abstract void Update();

    public void ConnectTo(Node target, int outId, int inId)
    {
        Network net;
        if (target.InputPins[inId].Network == null)
        {
            net = Owner.CreateNet();
            net.Add(target.InputPins[inId]);
        }
        else
        {
            net = target.InputPins[inId].Network;
        }

        net.Add(OutputPins[outId]);
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

        foreach (var pin in InputPins)
        {
            bounds.ExtendWith(pin.Bounds);
        }
        foreach (var pin in OutputPins)
        {
            bounds.ExtendWith(pin.Bounds);
        }

        Bounds = bounds;
    }

    public override WorldObj GetAt(PointF pos)
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
}


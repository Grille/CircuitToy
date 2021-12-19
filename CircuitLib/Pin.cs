using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;
using System.Numerics;

namespace CircuitLib;

public abstract class Pin : Entity
{
    public List<Wire> ConnectedWires = new List<Wire>();

    private Vector2 _pos;
    private Vector2 _rPos;
    public override Vector2 Position {
        set {
            _pos = value;
            _rPos = new Vector2(_pos.X - Owner.Position.X, _pos.Y - Owner.Position.Y);
            CalcBoundings();
        }
        get { 
            return _pos; 
        }
    }

    public Vector2 RelativePosition {
        set { 
            _rPos = value;
            _pos = new Vector2(Owner.Position.X + _rPos.X, Owner.Position.Y + _rPos.Y);
            CalcBoundings();
        }
        get { 
            return _rPos; 
        }
    }

    public abstract void ConnectTo(Pin pin1);
    public abstract void ConnectTo(Vector2 pin1);

    public override void Destroy()
    {
        DestroyConnections();
        base.Destroy();
    }

    public void DestroyConnections()
    {
        var refWires = new List<Wire>();
        foreach (var wire in ConnectedWires)
        {
            refWires.Add(wire);
        }

        foreach (var wire in refWires)
        {
            wire.Destroy();
        }
    }

    public void UpdatePosition()
    {
        _pos = new Vector2(Owner.Position.X + _rPos.X, Owner.Position.Y + _rPos.Y);
        CalcBoundings();
    }

    public Pin()
    {
    }

    public Pin(Entity owner) : this(owner, 0, 0) { }

    public Pin(Entity owner, float x, float y)
    {
        Owner = owner;
        RelativePosition = new Vector2(x,y);
    }

    public List<Pin> GetConnectedPins()
    {
        var list = new List<Pin>();
        list.Add(this);
        getConnectedPins(list);
        return list;
    }

    private void getConnectedPins(List<Pin> pins)
    {
        foreach (var wire in ConnectedWires)
        {
            if (wire.StartPin != this && !pins.Contains(wire.StartPin))
            {
                pins.Add(wire.StartPin);
                wire.StartPin.getConnectedPins(pins);
            }
            if (wire.EndPin != this && !pins.Contains(wire.EndPin))
            {
                pins.Add(wire.EndPin);
                wire.EndPin.getConnectedPins(pins);
            }
        }
    }

    public override void CalcBoundings()
    {
        Bounds = new BoundingBox(_pos, 0.35f);
        Owner.CalcBoundings();
        foreach (var wire in ConnectedWires)
        {
            wire.CalcBoundings();
        }
    }
}


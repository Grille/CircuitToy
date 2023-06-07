using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

using CircuitLib.IntMath;

namespace CircuitLib;

public abstract class IOPin : Pin
{
    
    public IOPin() : base() { }
    public IOPin(Node owner) : base(owner) { }
    public IOPin(Node owner, float x, float y) : base(owner, x, y) { }
    public IOPin(Node owner, Vector2 pos) : base(owner, pos) { }

    internal State _state = State.Off;

    public abstract State State {
        get;
        set;
    }

    public override Vector2 Position {
        set {
            _position = value;
            calcLineEnd();
            CalcBoundings();
        }
        get {
            return _position;
        }
    }

    public override Vector2 RelativePosition {
        set {
            _relativePosition = value;
            calcLineEnd();
            CalcBoundings();
        }
        get {
            return _relativePosition;
        }
    }

    public Vector2 LineEndPosition {
        private set; get;
    }

    public new Node Owner {
        get {
            return (Node)base.Owner;
        }
        set {
            base.Owner = value;
        }
    }

    public override void ConnectTo(Pin pin)
    {
        if (ConnectedNetwork == null)
        {
            var net = Owner.Owner.Networks.Create();
            net.Pins.Add(this);
        }

        ConnectedNetwork.ConnectFromTo(this, pin);
    }

    public override void ConnectTo(Vector2 pos)
    {
        if (ConnectedNetwork == null)
        {
            var net = Owner.Owner.Networks.Create();
            net.Pins.Add(this);
        }

        ConnectedNetwork.ConnectFromTo(this, pos);
    }

    public override void CalcBoundings()
    {
        Bounds = new BoundingBox(Position, 0.7f);
        base.CalcBoundings();
        ConnectedNetwork?.CalcBoundings();
    }

    public void UpdatePosition()
    {
        _position = new Vector2(Owner.Position.X + _relativePosition.X, Owner.Position.Y + _relativePosition.Y);
        calcLineEnd();
        CalcBoundings();
    }

    private void calcLineEnd()
    {
        Vector2 endpos = Vector2.Zero;

        if (Owner.Size.X / 2 < MathF.Abs(_relativePosition.X))
            endpos.Y = _position.Y;
        else
            endpos.Y = Owner.Position.Y;

        if (Owner.Size.Y / 2 < MathF.Abs(_relativePosition.Y))
            endpos.X = _position.X;
        else
            endpos.X = Owner.Position.X;

        LineEndPosition = endpos;
    }
}


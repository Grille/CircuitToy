using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

using CircuitLib.Math;

namespace CircuitLib;

public abstract class IOPin : Pin
{
    public IOPin() : base() { }
    public IOPin(Node owner) : base(owner) { }
    public IOPin(Node owner, float x, float y) : base(owner, x, y) { }

    internal State _state = State.Off;

    public abstract State State {
        get;
        set;
    }

    public override Vector2 Position {
        set {
            _pos = value;
            _rPos = new Vector2(_pos.X - Owner.Position.X, _pos.Y - Owner.Position.Y);
            calcLineEnd();
            CalcBoundings();
        }
        get {
            return _pos;
        }
    }

    public override Vector2 RelativePosition {
        set {
            _rPos = value;
            _pos = new Vector2(Owner.Position.X + _rPos.X, Owner.Position.Y + _rPos.Y);
            calcLineEnd();
            CalcBoundings();
        }
        get {
            return _rPos;
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

    public Network ConnectedNetwork;

    public override void ConnectTo(Pin pin)
    {
        if (ConnectedNetwork == null)
        {
            var net = Owner.Owner.CreateNet();
            net.Add(this);
        }

        ConnectedNetwork.ConnectFromTo(this, pin);
    }

    public override void ConnectTo(Vector2 pos)
    {
        if (ConnectedNetwork == null)
        {
            var net = Owner.Owner.CreateNet();
            net.Add(this);
        }

        ConnectedNetwork.ConnectFromTo(this, pos);
    }

    public override void CalcBoundings()
    {
        base.CalcBoundings();
        ConnectedNetwork?.CalcBoundings();
    }

    public override void Destroy()
    {
        ConnectedNetwork?.Remove(this);
        base.Destroy();
    }

    public void UpdatePosition()
    {
        _pos = new Vector2(Owner.Position.X + _rPos.X, Owner.Position.Y + _rPos.Y);
        calcLineEnd();
        CalcBoundings();
    }

    private void calcLineEnd()
    {
        Vector2 endpos = Vector2.Zero;

        if (Owner.Size.X / 2 < MathF.Abs(_rPos.X))
            endpos.Y = _pos.Y;
        else
            endpos.Y = Owner.Position.Y;

        if (Owner.Size.Y / 2 < MathF.Abs(_rPos.Y))
            endpos.X = _pos.X;
        else
            endpos.X = Owner.Position.X;

        LineEndPosition = endpos;
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;

namespace CircuitLib;

public abstract class Pin : Entity
{
    public Node Owner;
    public Network Network;

    public string Name = "";
    public string Description = "";

    private PointF _pos;
    private PointF _rPos;
    public override PointF Position {
        set {
            _pos = value;
            _rPos = new PointF(_pos.X - Owner.Position.X, _pos.Y - Owner.Position.Y);
            CalcBoundings();
        }
        get { 
            return _pos; 
        }
    }

    public PointF RelativePosition {
        set { 
            _rPos = value;
            _pos = new PointF(Owner.Position.X + _rPos.X, Owner.Position.Y + _rPos.Y);
            CalcBoundings();
        }
        get { 
            return _rPos; 
        }
    }

    public void UpdatePosition()
    {
        _pos = new PointF(Owner.Position.X + _rPos.X, Owner.Position.Y + _rPos.Y);
        CalcBoundings();
    }

    public Pin(Node owner) : this(owner, 0, 0) { }

    public Pin(Node owner, float x, float y)
    {
        Owner = owner;
        RelativePosition = new PointF(x,y);
    }

    public abstract bool Active {
        get; set;
    }

    public override void CalcBoundings()
    {
        Bounds = new BoundingBoxF(_pos, 0.35f);
        Owner.CalcBoundings();
    }
}


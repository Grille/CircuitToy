using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib;

public abstract class Pin : World
{
    public Node Owner;
    public Network Network;

    public string Name = "";
    public string Description = "";

    private PointF _pos;
    private PointF _rPos;
    public PointF Position {
        get { return _pos; }
    }

    public PointF RelativePosition {
        set { 
            _rPos = value;
            UpdatePosition();
        }
        get { return _rPos; }
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

    public void UpdatePosition()
    {
        _pos = new PointF(Owner.Position.X + _rPos.X, Owner.Position.Y + _rPos.Y);
        SetBoundings(_pos, 1);
    }

    public override float DistanceTo(PointF pos)
    {
        return 0;
    }
}


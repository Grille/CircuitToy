using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using CircuitLib.IntMath;

namespace CircuitLib;

public class WirePin : Pin
{
    public WirePin(Network owner) : base(owner) { }
    public WirePin(Network owner, float x, float y) : base(owner, x, y) { }
    public WirePin(Node owner, Vector2 pos) : base(owner, pos) { }

    public new Network Owner {
        get {
            return (Network)base.Owner;
        }
        set {
            base.Owner = value;
        }
    }

    public State State {
        get => Owner.State;
    }

    public bool IsJoint {
        get => ConnectedWires.Count != 2;
    }

    public override void ConnectTo(Pin pin)
    {
        Owner.ConnectFromTo(this, pin);
    }

    public override void ConnectTo(Vector2 pos)
    {
        Owner.ConnectFromTo(this, pos);
    }

    public override void CalcBoundings()
    {
        Bounds = new BoundingBox(Position, 0.5f);
        base.CalcBoundings();
    }

    public override string GetDebugStr()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Pin::{GetType().Name} ID[{ID}] N:{Name}");
        if (Owner != null)
            sb.Append(Owner.GetDebugStr());

        return sb.ToString();
    }
}


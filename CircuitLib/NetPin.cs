using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib;

public class NetPin : Pin
{
    public NetPin(Network owner) : base(owner) { }
    public NetPin(Network owner, float x, float y) : base(owner, x, y) { }

    public new Network Owner {
        get {
            return (Network)base.Owner;
        }
        set {
            base.Owner = value;
        }
    }

    private bool _active;
    public override bool Active {
        get => _active;
        set => _active = value;
    }

    public override void ConnectTo(Pin pin)
    {
        Owner.ConnectFromTo(this, pin);
    }

    public override void ConnectTo(Vector2 pos)
    {
        Owner.ConnectFromTo(this, pos);
    }

    public override void Destroy()
    {
        Owner?.Remove(this);
        base.Destroy();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;

namespace CircuitLib;

public abstract class IOPin : Pin
{
    public IOPin(Node owner) : base(owner) { }
    public IOPin(Node owner, float x, float y) : base(owner, x, y) { }

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

    public override void ConnectTo(PointF pos)
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
}


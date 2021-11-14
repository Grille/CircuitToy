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

    public override void ConnectTo(Pin pin1)
    {
        throw new NotImplementedException();
    }

    public Network ConnectedNetwork;

    public override void CalcBoundings()
    {
        base.CalcBoundings();
        ConnectedNetwork?.CalcBoundings();
    }
}


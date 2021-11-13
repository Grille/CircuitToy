using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib;

public class InputPin : Pin
{
    public InputPin(Node owner) : base(owner) { }
    public InputPin(Node owner, float x, float y) : base(owner, x, y) { }

    private bool _active;
    public override bool Active {
        get {
            return _active;
        }
        set {
            if (_active != value)
            {
                _active = value;
                Owner.Update();
            }
        }
    }
}


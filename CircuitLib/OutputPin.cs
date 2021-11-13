using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib;

public class OutputPin : Pin
{

    public OutputPin(Node owner) : base(owner) { }
    public OutputPin(Node owner, float x, float y) : base(owner, x, y) { }

    private bool _active;
    public override bool Active {
        get {
            return _active;
        }
        set {
            if (_active != value)
            {
                _active = value;
                Network?.Update();
            }
        }
    }
}


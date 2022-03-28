using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib;

public class InputPin : IOPin
{
    public InputPin() : base() { }
    public InputPin(Node owner) : base(owner) { }
    public InputPin(Node owner, float x, float y) : base(owner, x, y) { }
    public InputPin(Node owner, Vector2 pos) : base(owner, pos) { }

    public override State State {
        get {
            return _state;
        }
        set {
            if (_state != value)
            {
                _state = value;
            }
        }
    }
}


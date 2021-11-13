using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib;

public class InputPin
{
    public Node Owner;

    public string Name;
    public string Description;
    public Point Position;

    private bool _active;
    public bool Active {
        get {
            return _active;
        }
        set {
            if (_active != value)
            {
                _active = value;
                Owner?.Update();
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib.Primitives;

public class XorGate : Node
{
    public XorGate()
    {
        Name = "XOR";

        InputPins = new[] {
            new InputPin(this,-2,-1),
            new InputPin(this,-2,+1),
        };

        OutputPins = new[] {
            new OutputPin(this,+3,+0),
        };

        SetSize(3);
    }

    public override void Update()
    {
        OutputPins[0].Active = InputPins[0].Active ^ InputPins[1].Active;
    }
}


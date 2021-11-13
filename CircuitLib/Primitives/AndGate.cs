using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Primitives;

public class AndGate : Node
{
    public AndGate()
    {
        Name = "AND";

        InputPins = new[] {
            new InputPin(this,-2,-1),
            new InputPin(this,-2,+1),
        };

        OutputPins = new[] {
            new OutputPin(this,+3,+0),
        };

        Size = new SizeF(3,3);
    }

    public override void Update()
    {
        OutputPins[0].Active = InputPins[0].Active && InputPins[1].Active;
    }
}


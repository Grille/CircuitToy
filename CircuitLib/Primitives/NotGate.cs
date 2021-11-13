using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib.Primitives;

public class NotGate : Node
{
    public NotGate()
    {
        Name = "NOT";

        InputPins = new[] {
            new InputPin(),
        };

        OutputPins = new[] {
            new OutputPin(),
        };
    }

    public override void Update()
    {
        OutputPins[0].Active = !InputPins[0].Active;
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Primitives;

public class Output : Node
{
    public Output()
    {
        DisplayName = "OUT";
        
        InputPins = new[] {
            new InputPin(this,-2,+0),
        };

        OutputPins = new OutputPin[0];

        Size = new System.Numerics.Vector2(4, 2);
    }

    public override void Update()
    {
        Active = InputPins[0].Active;
    }
}


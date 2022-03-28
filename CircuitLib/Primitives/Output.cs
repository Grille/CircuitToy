using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib.Primitives;

public class Output : Node
{
    public Output()
    {
        DisplayName = "OUT";

        InitPins(
            new Vector2[] {
                new (-2,-0),
            },
            new Vector2[] {

            }
        );

        Size = new Vector2(4, 2);
    }

    public State State {
        get => InputPins[0].State;
    }

    protected override void OnUpdate()
    {
        //Active = InputPins[0].Active;
    }
}


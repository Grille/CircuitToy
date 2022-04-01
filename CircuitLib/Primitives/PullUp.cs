using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib.Primitives;

public class PullUp : Node
{
    public PullUp()
    {
        DisplayName = "P-UP";

        InitPins(
            new Vector2[] {
                new (-3,-0),
            },
            new Vector2[] {
                new (+3,+0)
            }
        );

        Size = new Vector2(6, 2);
    }

    protected override void OnUpdate()
    {
        PullInputValues();
        if (InputStateBuffer[0] == State.Off)
            OutputStateBuffer[0] = State.High;
        else
            OutputStateBuffer[0] = InputStateBuffer[0];
        SendOutputSignal(0);
    }
}


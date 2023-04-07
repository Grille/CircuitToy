using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib.Primitives;

public class TriState : Node
{
    public TriState()
    {
        DisplayName = "Tri-State";

        InitPins(
            new Vector2[] {
                new (-4, +0),
                new (0, -2),
            },
            new Vector2[] {
                new (+4, +0)
            }
        );

        Size = new Vector2(8, 4);
    }

    protected override void OnUpdate()
    {
        PullInputValues();
        OutputStateBuffer[0] = (InputStateBuffer[0], InputStateBuffer[1]) switch {
            (State.Low, State.High) => State.Low,
            (State.High, State.High) => State.High,
            (State.Low, State.Low) => State.Off,
            (State.High, State.Low) => State.Off,
            (_, _) => State.Error,
        };
        SendOutputSignal(0);
    }
}


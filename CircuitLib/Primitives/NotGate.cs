using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib.Primitives;

public class NotGate : Node
{
    public NotGate()
    {
        DisplayName = "NOT";

        InitPins(
            new Vector2[] {
                new (-2,-0),
            },
            new Vector2[] {
                new (+2,+0)
            }
        );

        Size = new Vector2(4, 2);
    }

    protected override void OnUpdate()
    {
        OutputPins[0].State = InputPins[0].State switch {
            State.Low => State.High,
            State.High => State.Low,
            _ => State.Off,
        };
    }
}


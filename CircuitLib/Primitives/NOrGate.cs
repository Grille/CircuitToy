using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib.Primitives;

public class NOrGate : Node
{
    public NOrGate()
    {
        DisplayName = "NOR";

        InitPins(
            new Vector2[] {
                new (-3,-1),
                new (-3,+1),
            },
            new Vector2[] {
                new (+3,+0)
            }
        );

        Size = new Vector2(6, 4);
    }

    protected override void OnUpdate()
    {
        OutputPins[0].State = (InputPins[0].State, InputPins[1].State) switch {
            (State.Low, State.Low) => State.High,
            (State.Low, State.High) => State.Low,
            (State.High, State.Low) => State.Low,
            (State.High, State.High) => State.Low,
            (State.Low, _) => State.High,
            (_, State.Low) => State.High,
            (State.High, _) => State.Low,
            (_, State.High) => State.Low,
            (_, _) => State.Off,
        };
    }
}


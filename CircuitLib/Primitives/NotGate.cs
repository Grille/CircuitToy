using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Primitives;

public class NotGate : Node
{
    public NotGate()
    {
        DisplayName = "NOT";

        InputPins = new[] {
            new InputPin(this,-2,+0),
        };

        OutputPins = new[] {
            new OutputPin(this,+2,+0),
        };

        Size = new System.Numerics.Vector2(4, 2);
    }

    public override void Update()
    {
        var oldState = OutputPins[0].State;

        OutputPins[0].State = InputPins[0].State switch {
            State.Low => State.High,
            State.High => State.Low,
            _ => State.Error,
        };

        if (oldState != OutputPins[0].State)
            OutputPins[0].ConnectedNetwork?.Update();
    }
}


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
        DisplayName = "AND";

        InputPins = new[] {
            new InputPin(this,-3,-1),
            new InputPin(this,-3,+1),
        };

        OutputPins = new[] {
            new OutputPin(this,+3,+0),
        };

        Size = new System.Numerics.Vector2(6, 4);
    }

    public override void Update()
    {
        var oldState = OutputPins[0].State;

        OutputPins[0].State = (InputPins[0].State, InputPins[1].State) switch {
            (State.Low, State.Low) => State.Low,
            (State.Low, State.High) => State.Low,
            (State.High, State.Low) => State.Low,
            (State.High, State.High) => State.High,
            (State.Low, _) => State.Low,
            (_, State.Low) => State.Low,
            (State.High, _) => State.Low,
            (_, State.High) => State.Low,
            (_, _) => State.Error,
        };

        if (oldState != OutputPins[0].State)
            OutputPins[0].ConnectedNetwork?.Update();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Primitives;

public class Input : Node
{
    public Input()
    {
        DisplayName = "IN";

        InputPins = new InputPin[0];

        OutputPins = new[] {
            new OutputPin(this,+2,+0),
        };

        Size = new System.Numerics.Vector2(4, 2);

        State = State.Off;
    }

    public State State {
        get => OutputPins[0].State;
        set {
            OutputPins[0].State = value;
        }
    }

    public override void Update()
    {
        OutputPins[0].ConnectedNetwork?.Update();
    }

    public override void ClickAction()
    {
        if (State == State.Low)
            State = State.High;

        else if (State == State.High)
            State = State.Low;

        else
            State = State.Low;

        Update();
    }
}


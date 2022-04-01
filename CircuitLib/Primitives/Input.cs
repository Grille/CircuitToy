using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib.Primitives;

public class Input : Node
{
    public Input()
    {
        DisplayName = "IN";

        InitPins(
            new Vector2[] {

            },
            new Vector2[] {
                new (+2,+0)
            }
        );

        Size = new Vector2(4, 2);

        State = State.Off;
    }

    public State State {
        get;
        set;
    }

    protected override void OnUpdate() 
    {
        OutputStateBuffer[0] = State;
        SendOutputSignal(0);
    }

    public override void ClickAction()
    {
        State = OutputPins[0].State;

        if (State == State.Low)
            State = State.High;

        else if (State == State.High)
            State = State.Low;

        else
            State = State.Low;

        Update();
    }
}


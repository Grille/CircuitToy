using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using CircuitLib.Interface;

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

        State = State.Low;
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

    public override void ClickAction(CircuitEditor editor)
    {
        if (editor.IsShiftKeyDown)
        {
            State = State.Off;
        }
        else
        {
            State = OutputPins[0].State switch {
                State.Low => State.High,
                State.High => State.Low,
                _ => State.Low,
            };
        }
        
        Update();
    }
}


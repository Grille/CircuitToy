﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib.Primitives;

public class XorGate : Node
{
    public XorGate()
    {
        DisplayName = "XOR";

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
        PullInputValues();
        OutputStateBuffer[0] = (InputStateBuffer[0], InputStateBuffer[1]) switch {
            (State.Low, State.Low) => State.Low,
            (State.Low, State.High) => State.High,
            (State.High, State.Low) => State.High,
            (State.High, State.High) => State.Low,
            (_, _) => State.Error,
        };
        SendOutputSignal(0);
    }
}


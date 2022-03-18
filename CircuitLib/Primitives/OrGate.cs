﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Primitives;

public class OrGate : Node
{
    public OrGate()
    {
        DisplayName = "OR";

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
        OutputPins[0].Active = InputPins[0].Active || InputPins[1].Active;
    }
}


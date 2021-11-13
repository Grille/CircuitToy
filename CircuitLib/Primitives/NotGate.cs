﻿using System;
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
        Name = "NOT";

        InputPins = new[] {
            new InputPin(this,-2,+0),
        };

        OutputPins = new[] {
            new OutputPin(this,+2,+0),
        };

        Size = new SizeF(2, 2);
    }

    public override void Update()
    {
        OutputPins[0].Active = !InputPins[0].Active;
    }
}

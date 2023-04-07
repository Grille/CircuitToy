using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using CircuitLib.Interface;

namespace CircuitLib;

public class InputPin : IOPin
{
    public InputPin() : base() { }
    public InputPin(Node owner) : base(owner) { }
    public InputPin(Node owner, float x, float y) : base(owner, x, y) { }
    public InputPin(Node owner, Vector2 pos) : base(owner, pos) { }

    public override State State {
        get {
            return _state;
        }
        set {
            if (_state != value)
            {
                _state = value;
            }
        }
    }

    public override void ClickAction(CircuitEditor editor)
    {
        if (editor.IsShiftKeyDown)
        {
            State = State.Off;
        }
        else
        {
            State = State switch {
                State.Low => State.High,
                State.High => State.Low,
                _ => State.Low,
            };
        }

        Owner.Update();
    }

    public override string GetDebugStr()
    {
        var sb = new StringBuilder();

        int index = Array.IndexOf(Owner.InputPins, this);

        sb.AppendLine($"Pin::{GetType().Name} ID[{ID}] N:{Name}");
        sb.AppendLine($"Buffer-Value: {Owner.InputStateBuffer[index]}");
        if (ConnectedNetwork != null)
            sb.Append(ConnectedNetwork.GetDebugStr());

        return sb.ToString();
    }
}


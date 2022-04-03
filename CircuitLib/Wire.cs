using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;
using System.Numerics;

namespace CircuitLib;

public class Wire : Entity
{
    public Pin StartPin;
    public Pin EndPin;

    public new Network Owner {
        get {
            return (Network)base.Owner;
        }
        set {
            base.Owner = value;
        }
    }

    public Wire(Network owner, Pin start, Pin end)
    {
        Owner = owner;
        StartPin = start;
        EndPin = end;
        StartPin.ConnectedWires.Add(this);
        EndPin.ConnectedWires.Add(this);
        CalcBoundings();
    }


    Vector2 _pos;
    public override Vector2 Position {
        get => new Vector2((StartPin.Position.X+EndPin.Position.X)/2f, (StartPin.Position.Y + EndPin.Position.Y) / 2f);
        set =>  _pos = value;
    }

    public State State {
        get => Owner.State;
    }

    public override void CalcBoundings()
    {
        const float margin = 0.2f;
        Bounds = new BoundingBox(
            MathF.Min(StartPin.Position.X, EndPin.Position.X) - margin,
            MathF.Min(StartPin.Position.Y, EndPin.Position.Y) - margin,
            MathF.Max(StartPin.Position.X, EndPin.Position.X) + margin,
            MathF.Max(StartPin.Position.Y, EndPin.Position.Y) + margin
        );
    }

    public override void Destroy()
    {
        StartPin.ConnectedWires.Remove(this);
        EndPin.ConnectedWires.Remove(this);
        if (EndPin.Owner == this)
            EndPin.Destroy();
        Owner.Remove(this);
    }

    public override Entity GetAt(Vector2 pos0)
    {
        if (Bounds.IsInside(pos0))
        {
            var pos1 = StartPin.Position;
            var pos2 = EndPin.Position;
            float a = (pos2.X - pos1.X) * (pos1.Y - pos0.Y) - (pos1.X - pos0.X) * (pos2.Y - pos1.Y);
            float b = MathF.Sqrt(MathF.Pow(pos2.X - pos1.X, 2) + MathF.Pow(pos2.Y - pos1.Y, 2));
            float distToLine = MathF.Abs(a / b);

            if (distToLine < 0.3f)
            {
                return this;
            }
        }
        return null;
    }

    public Pin InsertPinAt(Vector2 pos0)
    {
        var pin = Owner.CreatePin(pos0.Round());
        StartPin.ConnectTo(pin);
        EndPin.ConnectTo(pin);
        Destroy();
        return pin;
    }

    public override void GetFromArea(List<Entity> entities, BoundingBox region)
    {
        return;
    }

    public Pin GetOtherPin(Pin pin)
    {
        if (pin == StartPin)
        {
            return EndPin;
        }
        else if (pin == EndPin)
        {
            return StartPin;
        }
        else
        {
            throw new ArgumentException("Pin not connected to wire!");
        }
    }

    public override string GetDebugStr()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Wire::{Name}");
        if (Owner != null)
            sb.Append(Owner.GetDebugStr());

        return sb.ToString();
    }
}

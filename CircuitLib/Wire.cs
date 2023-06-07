using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.IntMath;
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
        Bounds = new BoundingBox(StartPin.Position, EndPin.Position, margin);
    }

    public override void Destroy()
    {
        base.Destroy();

        StartPin.ConnectedWires.Remove(this);
        EndPin.ConnectedWires.Remove(this);
        if (EndPin.Owner == this)
            EndPin.Destroy();

        Owner?.Wires.Remove(this);
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

    public override void GetFromArea(List<Entity> entities, BoundingBox region)
    {
        //if (Bounds.IsColliding(region))
        //    entities.Add(this);

        var pos1 = StartPin.Position;
        var pos2 = EndPin.Position;
        var rect = (RectangleF)region;

        if (IsLineIntersectingRectangle(pos1.X, pos1.Y, pos2.X, pos2.Y, rect.X,rect.Y, rect.Width, rect.Height))
           entities.Add(this);
    }

    static bool IsLineIntersectingRectangle(float x1, float y1, float x2, float y2, float rx, float ry, float rw, float rh)
    {
        float left = Math.Min(x1, x2);
        float right = Math.Max(x1, x2);
        float top = Math.Min(y1, y2);
        float bottom = Math.Max(y1, y2);

        // Check bounds
        if (left > rx + rw || right < rx || top > ry + rh || bottom < ry)
        {
            return false;
        }

        bool isLineStartInside = (x1 >= rx && x1 <= rx + rw && y1 >= ry && y1 <= ry + rh);
        bool isLineEndInside = (x2 >= rx && x2 <= rx + rw && y2 >= ry && y2 <= ry + rh);

        if (isLineStartInside && isLineEndInside)
        {
            return true; // Line is entirely inside the rectangle
        }


        float m = (y2 - y1) / (x2 - x1);

        float x;
        float y;

        // Check intersection with the left edge of the rectangle
        x = rx;
        y = m * (x - x1) + y1;
        if (y >= ry && y <= ry + rh && x >= left && x <= right)
        {
            return true;
        }

        // Check intersection with the right edge of the rectangle
        x = rx + rw;
        y = m * (x - x1) + y1;
        if (y >= ry && y <= ry + rh && x >= left && x <= right)
        {
            return true;
        }

        // Check intersection with the top edge of the rectangle
        y = ry;
        x = (y - y1) / m + x1;
        if (x >= rx && x <= rx + rw && y >= top && y <= bottom)
        {
            return true;
        }

        // Check intersection with the bottom edge of the rectangle
        y = ry + rh;
        x = (y - y1) / m + x1;
        if (x >= rx && x <= rx + rw && y >= top && y <= bottom)
        {
            return true;
        }

        return false;
    }

    public Pin InsertPinAt(Vector2 pos0)
    {
        var pin = Owner.Pins.Create(pos0.Round());
        StartPin.ConnectTo(pin);
        EndPin.ConnectTo(pin);
        Destroy();
        return pin;
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

        sb.AppendLine($"Wire::{GetType().Name} ID[{ID}] N:{Name}");
        if (Owner != null)
            sb.Append(Owner.GetDebugStr());

        return sb.ToString();
    }
}

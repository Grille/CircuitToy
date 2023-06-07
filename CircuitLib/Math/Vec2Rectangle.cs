using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace CircuitLib.IntMath;

public struct Vec2Rectangle
{
    public Vector2 Location;
    public Vector2 Size;

    public Vec2Rectangle(Vector2 pos, Vector2 size)
    {
        Location = pos;
        Size = size;
    }

    public static explicit operator Rectangle(Vec2Rectangle a) 
        => new Rectangle((int)a.Location.X, (int)a.Location.X, (int)a.Size.X, (int)a.Size.Y);

    public static implicit operator Vec2Rectangle(RectangleF a)
        => new Vec2Rectangle((Vector2)a.Location, (Vector2)a.Size);

    public static implicit operator RectangleF(Vec2Rectangle a)
        => new RectangleF((PointF)a.Location, (SizeF)a.Size);

    public static implicit operator BoundingBox(Vec2Rectangle a) 
        => new BoundingBox(a.Location, a.Location + a.Size);
}
 

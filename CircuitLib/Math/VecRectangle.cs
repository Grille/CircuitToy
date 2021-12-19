using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace CircuitLib.Math;

public struct VecRectangle
{
    Vector2 Location;
    Vector2 Size;

    public static explicit operator RectangleF(VecRectangle a) => new RectangleF((PointF)a.Location, (SizeF)a.Size);
    public static explicit operator Rectangle(VecRectangle a) => new Rectangle((int)a.Location.X, (int)a.Location.X, (int)a.Size.X, (int)a.Size.Y);
}


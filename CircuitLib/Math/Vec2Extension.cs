using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CircuitLib.IntMath;

internal static class Vec2Extension
{
    public static Vector2 Round(this Vector2 vec)
    {
        return new Vector2(MathF.Round(vec.X), MathF.Round(vec.Y));
    }
}

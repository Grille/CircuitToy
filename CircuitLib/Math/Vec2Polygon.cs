using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CircuitLib.Math;

public class Vec2Polygon : Vec2Path
{
    public Vec2Polygon(IList<Vector2> points) : base(points)
    {
    }
}


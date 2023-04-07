using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib.Shapes;

public class Polygon : Path
{
    public Polygon(IList<Vector2> points) : base(points)
    {
    }
}

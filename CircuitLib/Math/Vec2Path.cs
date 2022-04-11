using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using CircuitLib.Interface;

namespace CircuitLib.Math;

public class Vec2Path
{
    public readonly int Length;
    public readonly Vector2[] Points;
    public readonly BoundingBox Boundings;

    public Vec2Path(IList<Vector2> points)
    {
        Points = new Vector2[points.Count];
        Boundings = new BoundingBox();
        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];
            Points[i] = point;
            Boundings.ExtendWith(point);
        }
    }

    public Vec2Path Clone()
    {
        return new Vec2Path(Points);
    }

    public Vec2Path TransformToScreenSpace(Camera cam)
    {
        var points = new Vector2[Points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = cam.WorldToScreenSpace(Points[i]);
        }
        return new Vec2Path(points);
    }
}

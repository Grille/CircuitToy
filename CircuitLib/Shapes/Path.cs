using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Interface;
using CircuitLib.Math;
using CircuitLib.Rendering;

namespace CircuitLib.Shapes;

public class Path : Shape
{
    public readonly int Count;
    public readonly Vector2[] Points;

    public Path(IList<Vector2> points)
    {
        Points = new Vector2[points.Count];
        Count = points.Count;
        Bounds = new BoundingBox();
        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];
            Points[i] = point;
            Bounds.ExtendWith(point);
        }
        CalcBoundings();
    }

    public override Path Clone()
    {
        return new Path(Points);
    }

    public Path TransformToScreenSpace(Camera cam)
    {
        var points = new Vector2[Points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = cam.WorldToScreenSpace(Points[i]);
        }
        return new Path(points);
    }

    public override void CalcBoundings()
    {
        for (int i = 0; i <= Count; i++)
        {
            Bounds.ExtendWith(Points[i], LineWidth);
        }
    }

    public override bool IsColliding(Vector2 position)
    {
        throw new NotImplementedException();
    }

    public override bool IsColliding(BoundingBox bounds)
    {
        throw new NotImplementedException();
    }

    public override bool Draw(IRendererBackend ctx, int paint)
    {
        throw new NotImplementedException();
    }

    public override void Fill(IRendererBackend ctx, int paint)
    {
        throw new NotImplementedException();
    }
}

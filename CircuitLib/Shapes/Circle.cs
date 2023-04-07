using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;
using CircuitLib.Rendering;

namespace CircuitLib.Shapes;

internal class Circle : Shape
{
    public int Radius;

    public override void CalcBoundings()
    {
        Bounds = new BoundingBox(Position, Radius + LineWidth);
    }

    public override Circle Clone()
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

    public override bool IsColliding(Vector2 position)
    {
        throw new NotImplementedException();
    }

    public override bool IsColliding(BoundingBox bounds)
    {
        throw new NotImplementedException();
    }
}

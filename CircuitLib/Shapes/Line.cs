using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;
using CircuitLib.Rendering;

namespace CircuitLib.Shapes;

internal class Line : Shape
{
    readonly public Vector2 Begin;
    readonly public Vector2 End;

    public Line(Vector2 begin, Vector2 end)
    {
        Begin = begin;
        End = end;

        CalcBoundings();
    }

    public override void CalcBoundings()
    {
        throw new NotImplementedException();
    }

    public override Line Clone()
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

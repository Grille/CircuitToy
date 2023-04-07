using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;
using CircuitLib.Rendering;

namespace CircuitLib.Shapes;

public abstract class Shape
{
    public BoundingBox Bounds;
    public Vector2 Position;
    public int LineWidth;

    public abstract void CalcBoundings();
    public abstract bool IsColliding(Vector2 position);
    public abstract bool IsColliding(BoundingBox bounds);
    public abstract bool Draw(IRendererBackend ctx, int paint);

    public abstract void Fill(IRendererBackend ctx, int paint);

    public abstract Shape Clone();
}

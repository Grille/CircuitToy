using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace CircuitLib.Math;

public struct BoundingBox
{
    public float BeginX;
    public float EndX;
    public float BeginY;
    public float EndY;

    public BoundingBox()
    {
        BeginX = 0.0f;
        EndX = 0.0f;
        BeginY = 0.0f;
        EndY = 0.0f;
    }

    public BoundingBox(Vector2 pos, float radius)
    {
        BeginX = pos.X - radius;
        EndX = pos.X + radius;
        BeginY = pos.Y - radius;
        EndY = pos.Y + radius;
    }

    public BoundingBox(float beginX, float endX, float beginY, float endY)
    {
        BeginX = beginX;
        EndX = endX;
        BeginY = beginY;
        EndY = endY;
    }

    public void ExtendWith(BoundingBox bounds)
    {
        BeginX = MathF.Min(BeginX, bounds.BeginX);
        EndX = MathF.Max(EndX, bounds.EndX);
        BeginY = MathF.Min(BeginY, bounds.BeginY);
        EndY = MathF.Max(EndY, bounds.EndY);
    }

    public float getWidth()
    {
        return MathF.Abs(BeginX - EndX);
    }

    public float getHeight()
    {
        return MathF.Abs(BeginY - EndY);
    }

    public bool IsInside(Vector2 pos)
    {
        return pos.X >= BeginX && pos.X <= EndX && 
               pos.Y >= BeginY && pos.Y <= EndY;
    }

    public bool IsColliding(BoundingBox bounds)
    {
        return (BeginX <= bounds.EndX && EndX >= bounds.BeginX) && (BeginY <= bounds.EndY && EndY >= bounds.BeginY);
    }

    public static explicit operator RectangleF(BoundingBox b)
    {
        return new RectangleF(b.BeginX, b.BeginY, b.getWidth(), b.getHeight());
    }

   
}


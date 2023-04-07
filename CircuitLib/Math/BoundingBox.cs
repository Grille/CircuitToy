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

    public Vector2 Begin {
        get {
            return new Vector2(BeginX, BeginY);
        }
        set {
            BeginX = value.X; BeginY = value.Y;
        }
    }

    public Vector2 End {
        get {
            return new Vector2(EndX, EndY);
        }
        set {
            EndX = value.X; EndY = value.Y;
        }
    }

    public Vector2 Size {
        get {
            return new Vector2(Width, Height);
        }
    }

    public Vector2 Center {
        get => (Begin + End) / 2;
    }

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

    public BoundingBox(Vector2 begin, Vector2 end, float margin = 0)
    {
        BeginX = MathF.Min(begin.X, end.X) - margin;
        BeginY =  MathF.Min(begin.Y, end.Y) - margin;
        EndX = MathF.Max(begin.X, end.X) + margin;
        EndY = MathF.Max(begin.Y, end.Y) + margin;
    }

    public BoundingBox(float beginX, float beginY, float endX, float endY)
    {
        BeginX = beginX;
        BeginY = beginY;
        EndX = endX;
        EndY = endY;
    }

    public void ExtendWith(BoundingBox bounds)
    {
        BeginX = MathF.Min(BeginX, bounds.BeginX);
        BeginY = MathF.Min(BeginY, bounds.BeginY);
        EndX = MathF.Max(EndX, bounds.EndX);
        EndY = MathF.Max(EndY, bounds.EndY);
    }

    public void ExtendWith(Vector2 pos)
    {
        BeginX = MathF.Min(BeginX, pos.X);
        BeginY = MathF.Min(BeginY, pos.Y);
        EndX = MathF.Max(EndX, pos.X);
        EndY = MathF.Max(EndY, pos.Y);
    }

    public void ExtendWith(Vector2 pos, float margin)
    {
        ExtendWith(new BoundingBox(pos, margin));
    }

    public void AddMargin(float margin)
    {
        BeginX -= margin;
        BeginY -= margin;
        EndX += margin;
        EndY += margin;
    }

    public float Width => MathF.Abs(BeginX - EndX);
   
    public float Height => MathF.Abs(BeginY - EndY);
    

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
        => new RectangleF(b.BeginX, b.BeginY, b.Width, b.Height);

    public static implicit operator Vec2Rectangle(BoundingBox b)
        => new Vec2Rectangle(b.Begin, b.Size);

    public override string ToString()
    {
        return $"<{BeginX},{BeginY}><{EndX},{EndY}>";
    }



}


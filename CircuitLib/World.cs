using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib;

public abstract class World
{
    private float boundsBeginX = 0;
    private float boundsEndX = 0;
    private float boundsBeginY = 0;
    private float boundsEndY = 0;

    public bool Selected = false;

    public bool InsideBoundings(PointF pos)
    {
        return pos.X >= boundsBeginX && pos.X <= boundsEndX && pos.Y >= boundsBeginY && pos.Y <= boundsEndY;
    }

    public void SetBoundings(RectangleF rect)
    {
        boundsBeginX = rect.X;
        boundsEndX = rect.X + rect.Height;

        boundsBeginY = rect.Y;
        boundsEndY = rect.Y + rect.Height;
    }

    public void SetBoundings(PointF pos, float radius)
    {
        boundsBeginX = pos.X - radius;
        boundsEndX = pos.X + radius;

        boundsBeginY = pos.Y - radius;
        boundsEndY = pos.Y + radius;
    }

    public abstract float DistanceTo(PointF pos);
}


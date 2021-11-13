using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;

namespace CircuitLib;

public abstract class WorldObj
{
    internal BoundingBoxF Bounds;

    public bool Hover = false;
    public bool Selected = false;

    public abstract void CalcBoundings();

    public virtual WorldObj GetAt(PointF pos)
    {
        if (Bounds.IsInside(pos))
            return this;
        else
            return null;
    }
}


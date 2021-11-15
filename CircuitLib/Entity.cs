using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;
using System.IO;

namespace CircuitLib;

public abstract class Entity
{
    private Entity _owner;
    public Entity Owner {
        get {
            return _owner;
        }
        set {
            _owner = value;
        }
    }
    public BoundingBoxF Bounds;

    public bool IsAlive = true;
    public bool IsHovered = false;
    public bool IsSelected = false;

    public int ID = -1;
    public string Name = "";
    public string Description = "";

    public abstract PointF Position {
        get; set; 
    }
    public abstract void CalcBoundings();

    public virtual void Destroy()
    {
        IsAlive = false;
    }

    public virtual Entity GetAt(PointF pos)
    {
        if (Bounds.IsInside(pos))
            return this;
        else
            return null;
    }

    public virtual void GetFromArea(List<Entity> entities, BoundingBoxF region)
    {
        if (Bounds.IsColliding(region))
            entities.Add(this);
    }

    public virtual List<Entity> GetListFromArea(BoundingBoxF region)
    {
        var list = new List<Entity>();
        GetFromArea(list, region);
        return list;
    }

    public virtual void ClickAction()
    {

    }

    public void RoundPosition()
    {
        Position =  new PointF(MathF.Round(Position.X), MathF.Round(Position.Y));
    }

    public abstract bool Active {
        get; set;
    }
}


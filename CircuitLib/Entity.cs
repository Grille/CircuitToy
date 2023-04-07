using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;
using System.IO;
using System.Numerics;
using CircuitLib.Interface;

namespace CircuitLib;

/// <summary>
/// Object located in World
/// </summary>
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
    public BoundingBox Bounds;

    public bool IsEnabled = false;
    public bool IsAlive = true;
    public bool IsHovered = false;
    public bool IsVisible = true;
    public bool IsSelected = false;

    private static int globalID = 0;
    public int ID = globalID += 1;

    public string Name = "";
    public string Description = "";

    public abstract Vector2 Position {
        get; set; 
    }
    public abstract void CalcBoundings();

    public virtual void Destroy()
    {
        IsAlive = false;
    }

    public virtual Entity GetAt(Vector2 pos)
    {
        if (Bounds.IsInside(pos))
            return this;
        else
            return null;
    }

    public virtual void GetFromArea(List<Entity> entities, BoundingBox region)
    {
        if (Bounds.IsColliding(region))
            entities.Add(this);
    }

    public List<Entity> GetListFromArea(BoundingBox region)
    {
        var list = new List<Entity>();
        GetFromArea(list, region);
        return list;
    }

    public virtual void ClickAction(CircuitEditor inp)
    {

    }

    public void RoundPosition()
    {
        Position =  new Vector2(MathF.Round(Position.X), MathF.Round(Position.Y));
    }

    public abstract string GetDebugStr();
}


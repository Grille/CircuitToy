using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;

namespace CircuitLib.Interface;

public class Selection
{
    public Entity Hovered;
    public List<Entity> Selected;

    public Entity World;

    public PointF Offset;

    private PointF areaStart;
    private PointF areaEnd;

    public BoundingBoxF SelectetArea;
    public bool IsSelectingArea = false;

    public Selection(Entity world)
    {
        World = world;

        Hovered = null;
        Selected = new List<Entity>();

        Offset = PointF.Empty;
    }

    public Entity HoverAt(PointF pos)
    {
        var obj = World.GetAt(pos);
        if (obj == null)
        {
            if (Hovered != null)
            {
                Hovered.IsHovered = false;
                Hovered = null;
            }
        }
        else
        {
            if (Hovered != null && Hovered != obj)
            {
                Hovered.IsHovered = false;
            }
            Hovered = obj;
            Hovered.IsHovered = true;
        }
        return Hovered;
    }

    public void SelectAt(PointF pos)
    {
        Select(World.GetAt(pos));
    }

    public void ClickAt(PointF pos)
    {
        var obj = World.GetAt(pos);
        if (obj != null)
            obj.ClickAction();
    }

    public void Select(Entity obj)
    {
        if (obj != null && !obj.IsSelected)
        {
            obj.IsSelected = true;
            Selected.Add(obj);
        }
    }

    public void Select(List<Entity> entities)
    {
        foreach (var entity in entities)
        {
            Select(entity);
        }
    }

    public void DeselectAt(PointF pos)
    {
        var obj = World.GetAt(pos);
        if (obj != null && obj.IsSelected)
        {
            obj.IsSelected = false;
            Selected.Remove(obj);
        }
    }

    public void ToogleAt(PointF pos)
    {
        var obj = World.GetAt(pos);
        if (obj != null)
        {
            if (!obj.IsSelected)
            {
                obj.IsSelected = true;
                Selected.Add(obj);
            }
            else if (obj.IsSelected)
            {
                obj.IsSelected = false;
                Selected.Remove(obj);
            }
        }
    }

    public void SelectAreaBegin(PointF pos)
    {
        areaStart = pos;
        areaEnd = pos;
        SelectetArea = new BoundingBoxF(areaStart, 0);
        IsSelectingArea = true;
    }

    public void SelectAreaMove(PointF pos)
    {
        areaEnd = pos;

        float minX = MathF.Min(areaStart.X,areaEnd.X);
        float maxX = MathF.Max(areaStart.X,areaEnd.X);

        float minY = MathF.Min(areaStart.Y, areaEnd.Y);
        float maxY = MathF.Max(areaStart.Y, areaEnd.Y);

        SelectetArea = new BoundingBoxF(minX, maxX, minY, maxY);
    }

    public List<Entity> SelectAreaEnd()
    {
        IsSelectingArea = false;
        return World.GetListFromArea(SelectetArea);
    }

    public void ClearSelection()
    {
        foreach (var obj in Selected)
        {
            obj.IsSelected = false;
        }
        Selected.Clear();
    }

    public void ApplyOffset()
    {
        foreach (var obj in Selected)
        {
            obj.Position = new PointF(MathF.Round(obj.Position.X+Offset.X), MathF.Round(obj.Position.Y + Offset.Y));
        }
        Offset = PointF.Empty;
    }
}


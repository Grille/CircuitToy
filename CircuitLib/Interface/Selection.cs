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
    public Entity HoveredEntity;
    public List<Entity> SelectedEntities;

    public Entity Circuit;

    public PointF Offset;

    private PointF areaStart;
    private PointF areaEnd;

    public BoundingBoxF SelectetArea;
    public bool IsSelectingArea = false;

    public Selection(Entity world)
    {
        Circuit = world;

        HoveredEntity = null;
        SelectedEntities = new List<Entity>();

        Offset = PointF.Empty;
    }

    public Entity HoverAt(PointF pos)
    {
        var obj = Circuit.GetAt(pos);
        if (obj == null)
        {
            if (HoveredEntity != null)
            {
                HoveredEntity.IsHovered = false;
                HoveredEntity = null;
            }
        }
        else
        {
            if (HoveredEntity != null && HoveredEntity != obj)
            {
                HoveredEntity.IsHovered = false;
            }
            HoveredEntity = obj;
            HoveredEntity.IsHovered = true;
        }
        return HoveredEntity;
    }

    public void SelectAt(PointF pos)
    {
        Select(Circuit.GetAt(pos));
    }

    public void ClickAt(PointF pos)
    {
        var obj = Circuit.GetAt(pos);
        if (obj != null)
            obj.ClickAction();
    }

    public void Select(Entity obj)
    {
        if (obj == null)
            return;

        bool contain = SelectedEntities.Contains(obj);
        bool selected = obj.IsSelected;

        if (!selected && !contain)
        {
            obj.IsSelected = true;
            SelectedEntities.Add(obj);
        }
        else if (selected != contain)
            throw new InvalidOperationException();

        ProcessSeclection();
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
        var obj = Circuit.GetAt(pos);
        if (obj != null && obj.IsSelected)
        {
            obj.IsSelected = false;
            SelectedEntities.Remove(obj);
        }
    }

    public void ToogleAt(PointF pos)
    {
        var obj = Circuit.GetAt(pos);

        if (obj == null)
            return;

        bool contain = SelectedEntities.Contains(obj);
        bool selected = obj.IsSelected;

        if (contain != selected)
            throw new InvalidOperationException();

        if (!selected)
        {
            obj.IsSelected = true;
            SelectedEntities.Add(obj);
        }
        else if (selected)
        {
            obj.IsSelected = false;
            SelectedEntities.Remove(obj);
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

    public void SelectAreaEnd()
    {
        IsSelectingArea = false;
        SelectedEntities.Clear();

        var selection = Circuit.GetListFromArea(SelectetArea);

        foreach (var obj in selection)
        {
            Select(obj);
        }
        ProcessSeclection();
    }

    public void ClearSelection()
    {
        foreach (var obj in SelectedEntities)
        {
            obj.IsSelected = false;
        }
        SelectedEntities.Clear();
    }

    public void ApplyOffset()
    {
        Console.WriteLine("Apply");
        foreach (var obj in SelectedEntities)
        {
            bool apply = true;

            if (SelectedEntities.Contains(obj.Owner))
            {
                apply = false;
            }

            if (apply)
            {
                Console.WriteLine($"+ x{Offset.X},y{Offset.Y}");
                obj.Position = new PointF(obj.Position.X + Offset.X, obj.Position.Y + Offset.Y);
                obj.RoundPosition();
            }

        }
        Offset = PointF.Empty;
    }

    private void ProcessSeclection()
    {
        var list = new List<Entity>();
        foreach (var obj in SelectedEntities)
        {
            if (obj is Wire)
            {
                var wire = (Wire)obj;

                if (!wire.StartPin.IsSelected)
                {
                    wire.StartPin.IsSelected = true;
                    list.Add(wire.StartPin);
                }

                if (!wire.EndPin.IsSelected)
                {
                    wire.EndPin.IsSelected = true;
                    list.Add(wire.EndPin);
                }
            }
        }
        foreach (var obj in list)
        {
            SelectedEntities.Add(obj);
        }
    }

    public void Copy()
    {

    }

    public void Paste()
    {

    }
}


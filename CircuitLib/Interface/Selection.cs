using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;
using System.Numerics;

namespace CircuitLib.Interface;

public class Selection
{
    public Entity HoveredEntity;
    public List<Entity> SelectedEntities;
    private List<Entity> indirectSelection;

    public Entity Circuit;

    public Vector2 Offset;

    private Vector2 areaStart;
    private Vector2 areaEnd;

    public BoundingBox SelectetArea;
    public bool IsSelectingArea = false;

    public Selection(Entity world)
    {
        Circuit = world;

        HoveredEntity = null;
        SelectedEntities = new List<Entity>();
        indirectSelection = new List<Entity>();

        Offset = Vector2.Zero;
    }

    public Entity HoverAt(Vector2 pos)
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

    public void SelectAt(Vector2 pos)
    {
        Select(Circuit.GetAt(pos));
    }

    public void ClickAt(Vector2 pos)
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

    public void DeselectAt(Vector2 pos)
    {
        var obj = Circuit.GetAt(pos);
        if (obj != null && obj.IsSelected)
        {
            obj.IsSelected = false;
            SelectedEntities.Remove(obj);
        }
    }

    public void ToogleAt(Vector2 pos)
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

    public void SelectAreaBegin(Vector2 pos)
    {
        areaStart = pos;
        areaEnd = pos;
        SelectetArea = new BoundingBox(areaStart, areaEnd);
        IsSelectingArea = true;
    }

    public void SelectAreaMove(Vector2 pos)
    {
        areaEnd = pos;

        float minX = MathF.Min(areaStart.X, areaEnd.X);
        float maxX = MathF.Max(areaStart.X, areaEnd.X);

        float minY = MathF.Min(areaStart.Y, areaEnd.Y);
        float maxY = MathF.Max(areaStart.Y, areaEnd.Y);

        SelectetArea = new BoundingBox(minX, minY, maxX, maxY);
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
                obj.Position = new Vector2(obj.Position.X + Offset.X, obj.Position.Y + Offset.Y);
                obj.RoundPosition();
            }

        }
        foreach (var obj in indirectSelection)
        {
            bool apply = true;

            if (indirectSelection.Contains(obj.Owner))
            {
                apply = false;
            }

            if (apply)
            {
                Console.WriteLine($"+ x{Offset.X},y{Offset.Y}");
                obj.Position = new Vector2(obj.Position.X + Offset.X, obj.Position.Y + Offset.Y);
                obj.RoundPosition();
            }

        }

        Offset = Vector2.Zero;
    }

    private void ProcessSeclection()
    {
        indirectSelection.Clear();
        foreach (var obj in SelectedEntities)
        {
            if (obj is Wire)
            {
                var wire = (Wire)obj;

                selectIndirect(wire.StartPin);
                selectIndirect(wire.EndPin);
            }
        }

    }

    private void selectIndirect(Entity obj)
    {
        if (!SelectedEntities.Contains(obj) && !indirectSelection.Contains(obj))
        {
            indirectSelection.Add(obj);
        }
    }

    public void Copy()
    {

    }

    public void Paste()
    {

    }
}


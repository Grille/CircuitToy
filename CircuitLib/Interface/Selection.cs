using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CircuitLib.Math;
using System.Numerics;

namespace CircuitLib.Interface;

public enum SelectionMode
{
    Set,
    Add,
    Sub,
    Toogle,
}

public class Selection
{
    public List<Entity> HoveredEntities;
    public List<Entity> SelectedEntities;
    private List<Entity> indirectSelection;

    public Circuit Circuit;

    public Vector2 Offset;
    public Vector2 SnapOffset {
        get => Offset.Round();
    }

    public SelectionMode SingleMode = SelectionMode.Set;
    public SelectionMode AreaMode = SelectionMode.Set;

    private Vector2 areaStart;
    private Vector2 areaEnd;

    public BoundingBox SelectetArea;
    public bool IsSelectingArea = false;

    public Selection(Circuit world)
    {
        Circuit = world;

        HoveredEntities = new List<Entity>();
        SelectedEntities = new List<Entity>();
        indirectSelection = new List<Entity>();

        Offset = Vector2.Zero;
    }

    public Entity HoverAt(Vector2 pos)
    {
        ClearHoverd();

        var obj = Circuit.GetAt(pos);

        if (obj == null)
            return null;

        HoveredEntities.Add(obj);
        obj.IsHovered = true;
        return obj;
    }

    public List<Entity> HoverArea(BoundingBox area)
    {
        ClearHoverd();

        Circuit.GetFromArea(HoveredEntities, area);

        foreach (var entity in HoveredEntities)
        {
            entity.IsHovered = true;
        }

        return HoveredEntities;
    }

    public void ClearHoverd()
    {
        foreach (var obj in HoveredEntities)
        {
            obj.IsHovered = false;
        }
        HoveredEntities.Clear();
    }

    public void SelectAt(Vector2 pos)
    {
        var obj = Circuit.GetAt(pos);

        if (obj == null)
            return;

        switch (SingleMode)
        {
            case SelectionMode.Set:
                ClearSelection();
                Add(obj);
                break;

            case SelectionMode.Add:
                Add(obj);
                break;

            case SelectionMode.Sub:
                Remove(obj);
                break;

            case SelectionMode.Toogle:
                if (isSelected(obj))
                    Remove(obj);
                else
                    Add(obj);
                break;
        }

        ProcessSeclection();
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
        SelectetArea = new BoundingBox(areaStart, pos);
        HoverArea(SelectetArea);
    }

    public void SelectAreaEnd()
    {
        IsSelectingArea = false;

        var selection = Circuit.GetListFromArea(SelectetArea);

        ClearIndirectSelection();

        switch (AreaMode)
        {
            case SelectionMode.Set:
                ClearSelection();
                foreach (var obj in selection)
                {
                    Add(obj);
                }
                break;

            case SelectionMode.Add:
                foreach (var obj in selection)
                {
                    Add(obj);
                }
                break;

            case SelectionMode.Sub:
                foreach (var obj in selection)
                {
                    Remove(obj);
                }
                break;

            case SelectionMode.Toogle:
                foreach (var obj in selection)
                {
                    if (isSelected(obj))
                        Remove(obj);
                    else
                        Add(obj);
                }
                break;
        }

        ProcessSeclection();

        ClearHoverd();
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
        foreach (var obj in SelectedEntities)
        {
            bool apply = true;

            if (SelectedEntities.Contains(obj.Owner))
            {
                apply = false;
            }

            if (apply)
            {
                obj.Position = new Vector2(obj.Position.X + Offset.X, obj.Position.Y + Offset.Y);
                obj.RoundPosition();
            }

        }
        foreach (var obj in indirectSelection)
        {
            bool apply = true;

            if (SelectedEntities.Contains(obj.Owner))
            {
                apply = false;
            }

            if (apply)
            {
                obj.Position = new Vector2(obj.Position.X + Offset.X, obj.Position.Y + Offset.Y);
                obj.RoundPosition();
            }

        }

        Offset = Vector2.Zero;
    }

    public void ProcessSeclection()
    {
        ClearIndirectSelection();

        foreach (var obj in SelectedEntities)
        {
            switch (obj)
            {
                case Wire:
                {
                    var wire = (Wire)obj;
                    selectIndirect(wire.StartPin);
                    selectIndirect(wire.EndPin);
                    break;
                }
                case Node: {
                    var node = (Node)obj;
                    foreach (var pin in node.InputPins)
                    {
                        selectIndirect(pin);
                    }
                    foreach (var pin in node.OutputPins)
                    {
                        selectIndirect(pin);
                    }
                    break;
                }
            }
        }

    }

    private void ClearIndirectSelection()
    {
        foreach (var obj in indirectSelection)
        {
            obj.IsSelected = SelectedEntities.Contains(obj);
        }
        indirectSelection.Clear();
    }

    private void selectIndirect(Entity obj)
    {
        if (!SelectedEntities.Contains(obj) && !indirectSelection.Contains(obj))
        {
            obj.IsSelected = true;
            indirectSelection.Add(obj);
        }
    }

    internal bool isSelected(Entity obj)
    {
        bool selected = obj.IsSelected;
        bool contain = SelectedEntities.Contains(obj);

        if (selected != contain)
            throw new InvalidOperationException($"{obj} s:{selected} != c:{contain}");

        return selected;
    }
    internal void Add(Entity obj)
    {
        if (isSelected(obj))
            return;

        obj.IsSelected = true;
        SelectedEntities.Add(obj);
    }

    internal void Remove(Entity obj)
    {
        if (!isSelected(obj))
            return;

        obj.IsSelected = false;
        SelectedEntities.Remove(obj);
    }
}


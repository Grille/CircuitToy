using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Interface;

public class EditorInterface
{
    enum State
    {
        None,
        Selecting,
        Moving,
    }

    public Circuit Circuit;
    public Camera Camera;
    public Selection Selection;

    private State state = State.None;
    private bool isClick = false;

    public bool IsShiftKeyDown = false;
    public bool IsAltKeyDown = false;

    public PointF ScreenMousePos = Point.Empty;
    public PointF WorldMousePos = Point.Empty;
    public PointF WorldMouseDownPos = Point.Empty;
    public PointF WorldMouseUpPos = Point.Empty;

    public EditorInterface(Circuit circuit, Camera camera, Selection selection)
    {
        Circuit = circuit;
        Camera = camera;
        Selection = selection;  
    }

    public void MouseDown(PointF location, bool left)
    {
        WorldMouseDownPos = WorldMousePos;

        isClick = true;
        var obj = Circuit.GetAt(WorldMousePos);

        if (left)
        {
            if (obj == null)
            {
                if (!IsShiftKeyDown)
                    Selection.ClearSelection();
                Selection.SelectAreaBegin(WorldMousePos);

                isClick = false;
            }
            else if (obj.IsSelected)
            {

            }
            else
            {
                if (!IsShiftKeyDown)
                    Selection.ClearSelection();

                Selection.ToogleAt(WorldMousePos);
                isClick = false;
            }


            //Circuit.CreateNode<OrGate>(MathF.Round(pos.X), MathF.Round(pos.Y));
        }
    }

    public void MouseMove(PointF location, bool left)
    {
        ScreenMousePos = location;
        WorldMousePos = Camera.ScreenToWorldSpace(location);

        Entity entity = null;
        if (Selection.SelectedEntities.Count == 1)
            entity = Selection.SelectedEntities[0];

        if (left)
        {
            isClick = false;
            if (Selection.IsSelectingArea)
            {
                Selection.SelectAreaMove(WorldMousePos);
            }
            else
            {
                if (entity is Pin)
                {

                }
                else
                {
                    Selection.Offset = new PointF(WorldMousePos.X - WorldMouseDownPos.X, WorldMousePos.Y - WorldMouseDownPos.Y);
                }
            }
        }
        else
        {
            Selection.HoverAt(WorldMousePos);
        }
    }

    public void MouseUp(PointF location, bool left)
    {
        WorldMouseUpPos = WorldMousePos;

        Entity entity = null;
        if (Selection.SelectedEntities.Count == 1)
            entity = Selection.SelectedEntities[0];

        var obj = Circuit.GetAt(WorldMousePos);

        if (isClick)
        {
            if (left)
            {
                if (obj == null)
                {

                }
                else if (obj.IsSelected)
                {
                    if (!IsShiftKeyDown)
                        Selection.ClearSelection();

                    Selection.ToogleAt(WorldMousePos);
                }
                /*
                if (!isKeyDown(Keys.ShiftKey))
                {
                    Selection.ClearSelection();
                    Selection.ClickAt(pos);
                }
                */
            }
        }
        else
        {
            if (Selection.IsSelectingArea)
            {
                var list = Selection.SelectAreaEnd();
                Selection.Select(list);
            }
            else
            {
                if (entity is Pin)
                {
                    var pin = (Pin)entity;
                    pin.ConnectTo(WorldMousePos);
                }
                else
                {
                    Selection.ApplyOffset();
                }
            }
        }
    }

    public void KeyDown(int keycode)
    {

    }
}


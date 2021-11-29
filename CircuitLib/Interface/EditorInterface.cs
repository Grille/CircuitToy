using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Interface;

public enum ToolMode
{
    SelectAndMove,
    AddWire,
    OnOff,
}

public class EditorInterface
{
    public enum State
    {
        None,
        Selecting,
        Moving,
    }


    public Circuit Circuit;
    public Camera Camera;
    public Selection Selection;

    public ToolMode Mode = ToolMode.SelectAndMove;

    private State state = State.None;
    private bool isClick = false;

    public bool IsShiftKeyDown = false;
    public bool IsAltKeyDown = false;

    public PointF ScreenMousePos = Point.Empty;
    public PointF WorldMousePos = Point.Empty;
    public PointF WorldMouseDownPos = Point.Empty;
    public PointF WorldMouseUpPos = Point.Empty;

    public EditorInterface(Circuit circuit, Camera camera)
    {
        Circuit = circuit;
        Camera = camera;
        Selection = new Selection(circuit);  
    }

    public void MouseDown(PointF location, bool left)
    {
        WorldMouseDownPos = WorldMousePos;

        isClick = true;
        var obj = Circuit.GetAt(WorldMousePos);

        if (Mode == ToolMode.SelectAndMove)
        {
            if (left)
            {
                if (obj == null)
                {
                    if (!IsShiftKeyDown)
                        Selection.ClearSelection();
                    Selection.SelectAreaBegin(WorldMousePos);

                    isClick = false;
                }
                else if (!obj.IsSelected)
                {
                    if (!IsShiftKeyDown)
                        Selection.ClearSelection();

                    Selection.ToogleAt(WorldMousePos);
                    isClick = false;
                }
            }
        }
    }

    public void MouseMove(PointF location, bool left)
    {
        ScreenMousePos = location;
        WorldMousePos = Camera.ScreenToWorldSpace(location);

        if (left)
            isClick = false;

        if (Mode == ToolMode.SelectAndMove)
        {
            if (left)
            {
                


                if (Selection.IsSelectingArea)
                {
                    Selection.SelectAreaMove(WorldMousePos);
                }
                else if (Mode == ToolMode.SelectAndMove)
                {
                    Selection.Offset = new PointF(WorldMousePos.X - WorldMouseDownPos.X, WorldMousePos.Y - WorldMouseDownPos.Y);
                }

            }
        }
        Selection.HoverAt(WorldMousePos);
    }

    public void MouseUp(PointF location, bool left)
    {
        WorldMouseUpPos = WorldMousePos;

        var obj = Circuit.GetAt(WorldMousePos);

        if (Mode == ToolMode.SelectAndMove)
        {
            if (isClick)
            {
                if (left)
                {
                    if (obj != null && obj.IsSelected)
                    {
                        if (!IsShiftKeyDown)
                            Selection.ClearSelection();

                        Selection.ToogleAt(WorldMousePos);
                    }
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
                    Selection.ApplyOffset();
                }
            }
        }
        if (Mode == ToolMode.AddWire && !isClick)
        {
            var downObj = Circuit.GetAt(WorldMouseDownPos);
            var upObj = obj;

            Pin pin0 = null;
            Pin pin1 = null;

            if (downObj == null)
            {
                pin0 = Circuit.CreateNet().CreatePin(MathF.Round(WorldMouseDownPos.X), MathF.Round(WorldMouseDownPos.Y));
            }
            if (downObj is Pin)
            {
                pin0 = (Pin)downObj;
            }
            else if (downObj is Wire)
            {
                pin0 = ((Wire)downObj).InsertPinAt(WorldMouseDownPos);
            }

            if (upObj == null)
            {
                pin1 = Circuit.CreateNet().CreatePin(MathF.Round(WorldMouseUpPos.X), MathF.Round(WorldMouseUpPos.Y));
            }
            if (upObj is Pin)
            {
                pin1 = (Pin)upObj;
            }
            else if (upObj is Wire)
            {
                pin1 = ((Wire)upObj).InsertPinAt(WorldMouseUpPos);
            }

            pin0.ConnectTo(pin1);

        }
    }

    public void KeyDown(int keycode)
    {

    }

    public void Clear()
    {
        Selection.ClearSelection();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

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

    public Vector2 ScreenMousePos = Vector2.Zero;
    public Vector2 WorldMousePos = Vector2.Zero;
    public Vector2 WorldMouseDownPos = Vector2.Zero;
    public Vector2 WorldMouseUpPos = Vector2.Zero;

    public EditorInterface(Circuit circuit, Camera camera)
    {
        Circuit = circuit;
        Camera = camera;
        Selection = new Selection(circuit);  
    }

    public bool IsMoving {
        get {
            return state == State.Moving;
        }
    }

    public void MouseDown(Vector2 location, bool left)
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
                    //state = State.Selecting;

                    if (!IsShiftKeyDown)
                        Selection.ClearSelection();
                    Selection.SelectAreaBegin(WorldMousePos);

                    isClick = false;
                }
                else if (!obj.IsSelected)
                {
                    if (!IsShiftKeyDown)
                        Selection.ClearSelection();

                    Selection.SelectAt(WorldMousePos);
                    isClick = false;
                }
            }
        }
    }

    public void MouseMove(Vector2 location, bool left)
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
                    Selection.Offset = new Vector2(WorldMousePos.X - WorldMouseDownPos.X, WorldMousePos.Y - WorldMouseDownPos.Y);
                }

            }
        } 
        Selection.HoverAt(WorldMousePos);
    }

    public void MouseUp(Vector2 location, bool left)
    {
        WorldMouseUpPos = WorldMousePos;

        var obj = Circuit.GetAt(WorldMousePos);

        switch (Mode)
        {
            case ToolMode.SelectAndMove:
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
                        Selection.SelectAreaEnd();
                        //Selection.Select(Selection.SelectedEntities);
                    }
                    else
                    {
                        Selection.ApplyOffset();
                    }
                }
                break;
            }
            case ToolMode.AddWire:
            {
                if (!isClick)
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
                break;
            }
            case ToolMode.OnOff:
            {
                var downObj = Circuit.GetAt(WorldMouseDownPos);

                if (downObj != null)
                    downObj.ClickAction();

                break;
            }    
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


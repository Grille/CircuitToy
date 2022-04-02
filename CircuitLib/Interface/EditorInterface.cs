using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using CircuitLib.Math;

namespace CircuitLib.Interface;

public enum ToolMode
{
    SelectAndMove,
    AddWire,
    OnOff,
}

public enum EditorState
{
    None,
    Selecting,
    Moving,
    Wireing,
}

public class EditorInterface
{


    public Circuit Circuit;
    public Camera Camera;
    public Selection Selection;

    public ToolMode Mode = ToolMode.SelectAndMove;
    public EditorState State = EditorState.None;

    private bool isClick = false;

    public bool IsShiftKeyDown = false;
    public bool IsCtrlKeyDown = false;
    public bool IsAltKeyDown = false;

    public Vector2 ScreenMousePos = Vector2.Zero;
    public Vector2 WorldMousePos = Vector2.Zero;
    public Vector2 WorldMouseDownPos = Vector2.Zero;
    public Vector2 WorldMouseUpPos = Vector2.Zero;

    private int mouseMoveCounter = 0;

    public Entity DownEntity {
        private set; get; 
    }

    public Entity UpEntity {
        private set; get;
    }

    public EditorInterface(Circuit circuit, Camera camera)
    {
        Circuit = circuit;
        Camera = camera;
        Selection = new Selection(circuit);  
    }

    public bool IsMoving {
        get {
            return State == EditorState.Moving;
        }
    }

    public void MouseDown(Vector2 location, bool left)
    {
        WorldMouseDownPos = WorldMousePos;
        isClick = true;
        State = EditorState.None;
        mouseMoveCounter = 0;

        DownEntity = Circuit.GetAt(WorldMousePos);


        if (Mode == ToolMode.SelectAndMove)
        {
            switch (IsShiftKeyDown, IsCtrlKeyDown)
            {
                case (false, false):
                    Selection.SingleMode = SelectionMode.Set;
                    Selection.AreaMode = SelectionMode.Set;
                    break;

                case (false, true):
                    Selection.SingleMode = SelectionMode.Toogle;
                    Selection.AreaMode = SelectionMode.Sub;
                    break;

                case (true, false):
                    Selection.SingleMode = SelectionMode.Toogle;
                    Selection.AreaMode = SelectionMode.Add;
                    break;

                case (true, true):
                    Selection.SingleMode = SelectionMode.Toogle;
                    Selection.AreaMode = SelectionMode.Toogle;
                    break;
            }

            if (left)
            {
                //Console.WriteLine($"{Selection.SingleMode} {DownEntity.IsSelected}");
                if (DownEntity == null)
                {
                    Selection.SelectAreaBegin(WorldMousePos);
                    isClick = false;
                    State = EditorState.Selecting;
                }
                else if (Selection.SingleMode == SelectionMode.Set && DownEntity.IsSelected)
                {
                    Console.WriteLine("grap");
                    isClick = false;
                    State = EditorState.Moving;
                }
                else
                {
                    Console.WriteLine("select");
                    Selection.SelectAt(WorldMousePos);
                    isClick = false;
                    State = EditorState.Moving;
                }
            }
        }
        else if (Mode == ToolMode.AddWire)
        {
            if (left)
            {
                State = EditorState.Wireing;
            }
        }
    }

    public void MouseMove(Vector2 location, bool left)
    {
        ScreenMousePos = location;
        WorldMousePos = Camera.ScreenToWorldSpace(location);
        mouseMoveCounter++;
        bool firstMove = mouseMoveCounter == 1;

        bool doHover = true;

        if (left)
            isClick = false;

        if (Mode == ToolMode.SelectAndMove)
        {
            if (left)
            {

                Console.WriteLine(State);

                if (State == EditorState.Selecting)
                {
                    Selection.SelectAreaMove(WorldMousePos);
                    doHover = false;
                }
                else if (State == EditorState.Moving)
                {
                    Selection.Offset = new Vector2(WorldMousePos.X - WorldMouseDownPos.X, WorldMousePos.Y - WorldMouseDownPos.Y);
                }

            }
        } 

        if (doHover)
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
                switch (State)
                {
                    case EditorState.Selecting:
                        Selection.SelectAreaEnd();
                        break;

                    case EditorState.Moving:
                        Selection.ApplyOffset();
                        break;
                }

                break;
            }
            case ToolMode.AddWire:
            {
                if (!isClick)
                {
                    if (State == EditorState.Wireing)
                    {
                        if (pinPosValid(WorldMouseDownPos) && pinPosValid(WorldMouseUpPos))
                        {
                            var pin0 = getPin(WorldMouseDownPos);
                            var pin1 = getPin(WorldMouseUpPos);

                            pin0.ConnectTo(pin1);
                        }
                    }
                }
                break;
            }
            case ToolMode.OnOff:
            {
                var downObj = Circuit.GetAt(WorldMouseUpPos);

                if (left)
                    if (downObj != null)
                        downObj.ClickAction();

                break;
            }    
        }

        State = EditorState.None;

    }

    public void KeyDown(int keycode)
    {

    }

    public void DestroySelection()
    {
        foreach (var obj in Selection.SelectedEntities)
        {
            Console.WriteLine(obj.ToString());
            obj.Destroy();
        }
        Selection.ClearSelection();
    }

    public void CopySelection()
    {
    
    }

    public void CutSelection()
    {
        CopySelection();
        DestroySelection();
    }

    public void Paste(Vector2 pos)
    {

    }

    public void Clear()
    {
        Selection.ClearSelection();
    }

    bool pinPosValid(Vector2 pos)
    {
        var obj = Circuit.GetAt(pos);

        return obj switch {
            null => true,
            Pin => true,
            Wire => true,
            _ => false,
        };
    }

    Pin getPin(Vector2 pos)
    {
        var obj = Circuit.GetAt(pos);

        return obj switch {
            null => Circuit.CreateNet().CreatePin(pos.Round()),
            Pin => (Pin)obj,
            Wire => ((Wire)obj).InsertPinAt(pos),
            _ => null,
        };
    }
}


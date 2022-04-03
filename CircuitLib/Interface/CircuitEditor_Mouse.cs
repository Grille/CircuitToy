using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CircuitLib.Interface;

public partial class CircuitEditor
{
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
                    isClick = false;
                    State = EditorState.Moving;
                }
                else
                {
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
                        PushAction(new ActionApplyOffset());
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
                            BackupAction();

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
}

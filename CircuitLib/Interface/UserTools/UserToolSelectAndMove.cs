using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Interface.EditorActions;

namespace CircuitLib.Interface.UserTools;

public class UserToolSelectAndMove : UserActionHandler
{
    public UserToolSelectAndMove(CircuitEditor editor) : base(editor) { }

    enum ToolState
    {
        Selecting,
        Moving,
        None,
    }
    ToolState _state = ToolState.None;

    public override void MouseDown(EditorMouseArgs args)
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

        if (args.Left)
        {
            var DownEntity = Editor.Circuit.GetAt(WorldMousePos);
            if (DownEntity == null)
            {
                Selection.SelectAreaBegin(WorldMousePos);
                //isClick = false;
                _state = ToolState.Selecting;
            }
            else if (Selection.SingleMode == SelectionMode.Set && DownEntity.IsSelected)
            {
                //isClick = false;
                _state = ToolState.Moving;
            }
            else
            {
                Selection.SelectAt(WorldMousePos);
                //isClick = false;
                _state = ToolState.Moving;
            }
        }
    }

    public override void MouseUp(EditorMouseArgs args)
    {
        
        switch (_state)
        {
            case ToolState.Selecting:
                Selection.SelectAreaEnd();
                break;

            case ToolState.Moving:
                Editor.PushAction(new ActionApplyOffset());
                break;
        }
    }

    public override void MouseMove(EditorMouseArgs args)
    {
        if (args.Left)
        {
            if (_state == ToolState.Selecting)
            {
                Selection.SelectAreaMove(WorldMousePos);
                Selection.ClearHoverd();
                Selection.HoverArea(Selection.SelectetArea);
            }
            else if (_state == ToolState.Moving)
            {
                Selection.Offset = new Vector2(WorldMousePos.X - WorldMouseDownPos.X, WorldMousePos.Y - WorldMouseDownPos.Y);
            }

        }
        else
        {
            Selection.ClearHoverd();
            Selection.HoverAt(WorldMousePos);
        }
    }
}

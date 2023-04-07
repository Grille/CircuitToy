using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Interface.EditorActions;

namespace CircuitLib.Interface.EditorTools;

public abstract class EditorTool
{
    protected CircuitEditor Editor;
    protected bool IsShiftKeyDown => Editor.IsShiftKeyDown;
    protected bool IsCtrlKeyDown => Editor.IsCtrlKeyDown;
    protected bool IsAltKeyDown => Editor.IsAltKeyDown;
    protected Selection Selection => Editor.Selection;

    protected Vector2 WorldMousePos => Editor.WorldMousePos;
    protected Vector2 WorldMouseDownPos => Editor.WorldMouseDownPos;
    protected Vector2 WorldMouseUpPos => Editor.WorldMouseUpPos;


    public EditorTool(CircuitEditor editor)
    {
        Editor = editor;
    }

    public abstract void MouseDown(EditorMouseArgs args);

    public abstract void MouseUp(EditorMouseArgs args);

    public abstract void MouseMove(EditorMouseArgs args);

}

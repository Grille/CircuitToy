using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Interface.EditorActions;

namespace CircuitLib.Interface.EditorTools;

public class ToolOnOff : EditorTool
{
    public ToolOnOff(CircuitEditor editor) : base(editor) { }


    public override void MouseDown(EditorMouseArgs args)
    {

    }

    public override void MouseUp(EditorMouseArgs args)
    {
        var downObj = Editor.Circuit.GetAt(WorldMouseUpPos);

        if (args.Left)
            if (downObj != null)
                downObj.ClickAction(Editor);
    }

    public override void MouseMove(EditorMouseArgs args)
    {
        Selection.ClearHoverd();
        Selection.HoverAt(WorldMousePos);
    }

}

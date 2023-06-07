using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Interface.EditorActions;
using CircuitLib.IntMath;

namespace CircuitLib.Interface.UserTools;

public class UserToolAddWire : UserActionHandler
{
    public UserToolAddWire(CircuitEditor editor) : base(editor) { }

    public bool Wireing = false;
    public bool Click = false;
    public bool Error = false;

    public override void MouseDown(EditorMouseArgs args)
    {
        if (args.Left)
        {
            Wireing = true;
            Click = true;
            Error = false;
        }
    }

    public override void MouseUp(EditorMouseArgs args)
    {
        if (!Click && Wireing == true)
        {
            var begin = WorldMouseDownPos;
            var end = WorldMouseUpPos;

            if (Editor.PinPosValid(begin) && Editor.PinPosValid(end))
            {
                Editor.BackupAction();

                var pin0 = Editor.GetOrCreatePin(begin);
                var pin1 = Editor.GetOrCreatePin(end);

                pin0.ConnectTo(pin1);
            }

            Selection.ClearHoverd();
        }

        Wireing = false;
    }

    public override void MouseMove(EditorMouseArgs args)
    {
        Click = false;

        if (Wireing)
        {
            var beginpos = WorldMouseDownPos;
            var endpos = WorldMousePos;

            bool beginval = Editor.PinPosValid(beginpos);
            bool endval = Editor.PinPosValid(endpos);

            Selection.ClearHoverd();
            if (beginval)
                Selection.HoverAt(beginpos);
            if (endval)
                Selection.HoverAt(endpos);

            Error = !(beginval && endval);
        }
        else
        {
            var pos = WorldMousePos;
            bool val = Editor.PinPosValid(pos);
            Selection.ClearHoverd();
            if (val)
                Selection.HoverAt(pos);
        }
    }
}

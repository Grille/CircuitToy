using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CircuitLib.Interface;

internal class ActionApplyOffset : EditorAction
{
    protected override void OnExecute()
    {
        Editor.Selection.ApplyOffset();
    }
}

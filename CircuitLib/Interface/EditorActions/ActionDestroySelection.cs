using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib.Interface.EditorActions;

internal class ActionDestroySelection : EditorAction
{
    protected override void OnExecute()
    {
        foreach (var obj in Editor.Selection.SelectedEntities)
            obj.Destroy();
        Editor.Selection.ClearSelection();
    }
}

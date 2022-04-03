using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib.Interface;

internal class ActionDestroySelection : EditorAction
{
    protected override void OnExecute()
    {
        Console.WriteLine("onexecute");
        foreach (var obj in Editor.Selection.SelectedEntities)
        {
            obj.Destroy();
        }
        Editor.Selection.ClearSelection();
    }
}

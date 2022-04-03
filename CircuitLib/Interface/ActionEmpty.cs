using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib.Interface;

internal class ActionEmpty : EditorAction
{
    protected override void OnExecute()
    {
        return;
    }
}

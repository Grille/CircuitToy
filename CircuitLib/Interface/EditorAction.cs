using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib.Interface;

public abstract class EditorAction
{
    private byte[] _data;
    public CircuitEditor Editor { get; set; }

    public void Execute()
    {
        _data = Editor.CreateBackup();
        OnExecute();
    }

    protected abstract void OnExecute();

    public void Restore()
    {
        var data = _data;
        _data = Editor.CreateBackup();
        Editor.RestoreBackup(data);
    }

    public void Reverse()
    {
        var data = _data;
        _data = Editor.CreateBackup();
        Editor.RestoreBackup(data);
    }
}

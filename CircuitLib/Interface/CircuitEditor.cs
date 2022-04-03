using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using CircuitLib.Math;
using CircuitLib.Serialization;
using System.IO;
using GGL.IO;
using CircuitLib;

namespace CircuitLib.Interface;

public enum ToolMode
{
    SelectAndMove,
    AddWire,
    OnOff,
}

public enum EditorState
{
    None,
    Selecting,
    Moving,
    Wireing,
}

public partial class CircuitEditor
{


    public Circuit Circuit;
    public Camera Camera;
    public Selection Selection;

    public ToolMode Mode = ToolMode.SelectAndMove;
    public EditorState State = EditorState.None;

    Stack<EditorAction> PerformedStack = new Stack<EditorAction>();
    Stack<EditorAction> RecallStack = new Stack<EditorAction>();

    private bool isClick = false;

    public bool IsShiftKeyDown = false;
    public bool IsCtrlKeyDown = false;
    public bool IsAltKeyDown = false;

    public Vector2 ScreenMousePos = Vector2.Zero;
    public Vector2 WorldMousePos = Vector2.Zero;
    public Vector2 WorldMouseDownPos = Vector2.Zero;
    public Vector2 WorldMouseUpPos = Vector2.Zero;

    private int mouseMoveCounter = 0;
    private byte[] internalClipboard;

    public Entity DownEntity {
        private set; get; 
    }

    public Entity UpEntity {
        private set; get;
    }

    public CircuitEditor(Circuit circuit, Camera camera)
    {
        Circuit = circuit;
        Camera = camera;
        Selection = new Selection(circuit);  
    }

    public bool IsMoving {
        get {
            return State == EditorState.Moving;
        }
    }



    public void KeyDown(int keycode)
    {

    }

    public void DestroySelection()
    {
        PushAction(new ActionDestroySelection());
    }

    public byte[] CopySelection()
    {
        using var stream = new MemoryStream();
        using var bw = new BinaryViewWriter(stream);

        SerializatioUtils.WriteClipboard(bw, Selection.SelectedEntities);

        var bytes = stream.ToArray();

        internalClipboard = bytes;
        return bytes;
    }

    public byte[] CutSelection()
    {
        var result = CopySelection();
        DestroySelection();
        return result;
    }

    public void Paste(byte[] buffer)
    {
        BackupAction();

        using var stream = new MemoryStream(buffer);
        using var br = new BinaryViewReader(stream);

        var selection = DeserializationUtils.ReadClipboardToCircuit(br, Circuit);

        Selection.ClearSelection();

        foreach (var obj in selection)
        {
            Selection.Add(obj);
            obj.Position += WorldMousePos.Round();

            if (obj is IOPin)
            {
                var pin = (IOPin)obj;
                pin.ConnectedNetwork.Reset(CircuitLib.State.Off);
            }
            if (obj is NetPin)
            {
                var pin = (NetPin)obj;
                pin.Owner.Reset(CircuitLib.State.Off);
            }
            if (obj is Node)
            {
                var node = (Node)obj;
                node.Reset(CircuitLib.State.Off);
                node.Update();
            }
        }

        Selection.ProcessSeclection();
    }

    public void Paste()
    {
        Paste(internalClipboard);
    }

    public void Clear()
    {
        Selection.ClearSelection();
    }

    public void Redo()
    {
        if (RecallStack.Count == 0)
            return;
        var action = RecallStack.Pop();
        action.Restore();
        PerformedStack.Push(action);
    }

    public void Undo()
    {
        if (PerformedStack.Count == 0)
            return;
        var action = PerformedStack.Pop();
        action.Reverse();
        RecallStack.Push(action);
    }

    public byte[] CreateBackup()
    {
        using var stream = new MemoryStream();
        using var bw = new BinaryViewWriter(stream);

        var types = SerializatioUtils.WriteNodeTypes(bw, Circuit);
        SerializatioUtils.WriteCircuit(bw, Circuit, types);

        var bytes = stream.ToArray();

        return bytes;
    }

    public void RestoreBackup(byte[] buffer)
    {
        Console.WriteLine("restore "+buffer.Length);
        using var stream = new MemoryStream(buffer);
        using var br = new BinaryViewReader(stream);

        Selection.ClearSelection();
        Circuit.Destroy();

        var types = DeserializationUtils.ReadNodeTypes(br);
        DeserializationUtils.ReadToCircuit(br, Circuit, types);
    }

    public void BackupAction()
    {
        PushAction(new ActionEmpty());
    }

    public void PushAction<T>(T action) where T : EditorAction, new()
    {
        action.Editor = this;
        action.Execute();
        PerformedStack.Push(action);
        RecallStack.Clear();
    }

    bool pinPosValid(Vector2 pos)
    {
        var obj = Circuit.GetAt(pos);

        return obj switch {
            null => true,
            Pin => true,
            Wire => true,
            _ => false,
        };
    }

    Pin getPin(Vector2 pos)
    {
        var obj = Circuit.GetAt(pos);

        return obj switch {
            null => Circuit.Networks.Create().CreatePin(pos.Round()),
            Pin => (Pin)obj,
            Wire => ((Wire)obj).InsertPinAt(pos),
            _ => null,
        };
    }

    public void CreateNode<T>() where T : Node, new()
    {
        BackupAction();
        Circuit.Nodes.Create<T>(WorldMousePos);
    }
}


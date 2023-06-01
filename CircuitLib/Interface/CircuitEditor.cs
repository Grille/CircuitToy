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
using CircuitLib.Interface.EditorActions;
using CircuitLib.Interface.EditorTools;

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
    WireingError,
}

public partial class CircuitEditor
{


    public Circuit Circuit;
    public Camera Camera;
    public Selection Selection;

    private ToolMode _mode;

    public ToolMode Mode {
        get => _mode; 
        set {
            if (_mode == value)
                return;
            _mode = value;
            switch (_mode)
            {
                case ToolMode.SelectAndMove:
                    Tool = new ToolSelectAndMove(this);
                    break;
                case ToolMode.AddWire:
                    Tool = new ToolAddWire(this);
                    break;
                case ToolMode.OnOff:
                    Tool = new ToolOnOff(this);
                    break;
            }
        }

    }

    Stack<EditorAction> PerformedStack = new Stack<EditorAction>();
    Stack<EditorAction> RecallStack = new Stack<EditorAction>();

    public EditorTool Tool;

    public bool IsShiftKeyDown = false;
    public bool IsCtrlKeyDown = false;
    public bool IsAltKeyDown = false;

    public Vector2 ScreenMousePos = Vector2.Zero;
    public Vector2 WorldMousePos = Vector2.Zero;
    public Vector2 WorldMouseDownPos = Vector2.Zero;
    public Vector2 WorldMouseUpPos = Vector2.Zero;

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
        Tool = new ToolAddWire(this);
    }

    public void DestroySelection()
    {
        PushAction(new ActionDestroySelection());
    }

    public byte[] CopySelection()
    {
        using var stream = new MemoryStream();
        using var bw = new BinarySerializer(stream);

        bw.WriteClipboard(Selection.SelectedEntities);

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
        using var br = new BinaryDeserializer(stream);

        var selection = br.ReadClipboardToCircuit(Circuit);

        Selection.ClearSelection();

        foreach (var obj in selection)
        {
            Selection.Add(obj);
            obj.Position += WorldMousePos.Round();

            if (obj is IOPin)
            {
                var pin = (IOPin)obj;
                pin.ConnectedNetwork.Reset(State.Off);
            }
            if (obj is WirePin)
            {
                var pin = (WirePin)obj;
                pin.Owner.Reset(State.Off);
            }
            if (obj is Node)
            {
                var node = (Node)obj;
                node.Reset(State.Off);
                node.Update();
            }
        }

        Selection.ProcessSelection();
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
        BinarySerializer.WriteCircuit(stream, Circuit);
        var bytes = stream.ToArray();

        return bytes;
    }

    public void RestoreBackup(byte[] buffer)
    {
        Selection.ClearSelection();
        Circuit.Destroy();

        using var stream = new MemoryStream(buffer);
        BinaryDeserializer.ReadToCircuit(stream, Circuit);
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

    public bool PinPosValid(Vector2 pos)
    {
        var obj = Circuit.GetAt(pos);

        return obj switch {
            null => true,
            Pin => true,
            Wire => true,
            _ => false,
        };
    }

    public Pin GetOrCreatePin(Vector2 pos)
    {
        var obj = Circuit.GetAt(pos);

        return obj switch {
            null => Circuit.Networks.Create().Pins.Create(pos.Round()),
            Pin => (Pin)obj,
            Wire => ((Wire)obj).InsertPinAt(pos),
            _ => throw new InvalidOperationException(),
        };
    }

    public void CreateNode<T>() where T : Node, new()
    {
        BackupAction();
        Circuit.Nodes.Create<T>(WorldMousePos);
    }

    public void MouseDown(EditorMouseArgs args)
    {
        WorldMouseDownPos = WorldMousePos;

        Tool.MouseDown(args);
    }

    public void MouseMove(EditorMouseArgs args)
    {
        ScreenMousePos = args.Location;
        WorldMousePos = Camera.ScreenToWorldSpace(args.Location);

        Tool.MouseMove(args);
        //if (doHover)
        //    Selection.HoverAt(WorldMousePos);
    }

    public void MouseUp(EditorMouseArgs args)
    {
        WorldMouseUpPos = WorldMousePos;

        Tool.MouseUp(args);
    }
}


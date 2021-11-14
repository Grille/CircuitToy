using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using CircuitLib;
using CircuitLib.Primitives;
using CircuitLib.Interface;

namespace CircuitToy;

internal class Simulation
{
    public Circuit Circuit;
    public Camera Camera;
    public Renderer Renderer;
    public Control Target;
    public Selection Selection;

    Point lastLocation = Point.Empty;
    PointF mouseDownPos = Point.Empty;

    public ContextMenuStrip ContextMenu;
    public PointF MouseUpPos = Point.Empty;

    public Dictionary<Keys,bool> KeysDict = new Dictionary<Keys,bool>();

    
    bool isClick = false;

    public Simulation(Control target)
    {
        Target = target;

        Circuit = new Circuit();
        {
            var input0 = Circuit.CreateNode<Input>(0, 0);
            var input1 = Circuit.CreateNode<Input>(0, 4);
            var output = Circuit.CreateNode<Output>(10, 2);
            var orgate = Circuit.CreateNode<XorGate>(5, 2);

            input0.ConnectTo(orgate, 0, 0);
            input1.ConnectTo(orgate, 0, 1);
            orgate.ConnectTo(output, 0, 0);

            input0.Active = true;
            input0.Update();

            input1.Active = true;
            input1.Update();
        }



        Selection = new Selection(Circuit);

        Camera = new Camera();
        Renderer = new Renderer(Target, Circuit, Camera, Selection);
        Renderer.Start();

        Target.MouseMove += Target_MouseMove;
        Target.MouseWheel += Target_MouseWheel;
        Target.MouseDown += Target_MouseDown;
        Target.MouseUp += Target_MouseUp;

        Target.KeyDown += Target_KeyDown;



    }

    private void Target_KeyDown(object? sender, KeyEventArgs e)
    {
        throw new NotImplementedException();
    }

    void add()
    {
        Circuit.CreateNode<OrGate>(5, 2);
    }


    private void Target_MouseWheel(object? sender, MouseEventArgs e)
    {
        Camera.MouseScrollEvent(e, 1.5f);
    }

    private void Target_MouseMove(object? sender, MouseEventArgs e)
    {
        Camera.MouseMoveEvent(e, e.Button.HasFlag(MouseButtons.Middle));
        var pos = Camera.ScreenToWorldSpace(e.Location);
        
        float deltaX = (e.Location.X - lastLocation.X)/Camera.Scale;
        float deltaY = (e.Location.Y - lastLocation.Y) / Camera.Scale;

        if (e.Button == MouseButtons.Left)
        {
            isClick = false;
            if (Selection.IsSelectingArea)
            {
                Selection.SelectAreaMove(pos);
            }
            else
            {
                Selection.Offset = new PointF(pos.X - mouseDownPos.X, pos.Y - mouseDownPos.Y);
            }
        }
        else
        {
            Selection.HoverAt(pos);
        }

        lastLocation = e.Location;
    }

    private void Target_MouseDown(object? sender, MouseEventArgs e)
    {
        isClick = true;
        var pos = Camera.ScreenToWorldSpace(e.Location);
        mouseDownPos = pos;

        var obj = Circuit.GetAt(pos);

        if (e.Button == MouseButtons.Left)
        {
            if (obj == null)
            {
                if (!isKeyDown(Keys.ShiftKey))
                    Selection.ClearSelection();
                Selection.SelectAreaBegin(pos);
                isClick = false;
            }
            else if (obj.IsSelected)
            {

            }
            else
            {
                if (!isKeyDown(Keys.ShiftKey))
                    Selection.ClearSelection();

                Selection.ToogleAt(pos);
                isClick = false;
            }


            //Circuit.CreateNode<OrGate>(MathF.Round(pos.X), MathF.Round(pos.Y));
        }
    }

    private void Target_MouseUp(object? sender, MouseEventArgs e)
    {
        var pos = Camera.ScreenToWorldSpace(e.Location);
        MouseUpPos = pos;

        var obj = Circuit.GetAt(pos);

        if (isClick)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (obj == null)
                {

                }
                else if (obj.IsSelected)
                {
                    if (!isKeyDown(Keys.ShiftKey))
                        Selection.ClearSelection();

                    Selection.ToogleAt(pos);
                }
                /*
                if (!isKeyDown(Keys.ShiftKey))
                {
                    Selection.ClearSelection();
                    Selection.ClickAt(pos);
                }
                */
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenu.Show(Target, e.Location);
            }
        }
        else
        {
            if (Selection.IsSelectingArea)
            {
                var list = Selection.SelectAreaEnd();
                Selection.Select(list);
            }
            else
            {
                Selection.ApplyOffset();
            }
        }
    }

    bool isKeyDown(Keys keys)
    {
        if (KeysDict.ContainsKey(keys))
            return KeysDict[keys];
        return false;
    }
}

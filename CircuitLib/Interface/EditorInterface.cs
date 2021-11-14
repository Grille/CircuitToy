using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Interface;

public class EditorInterface
{
    public Circuit Circuit;
    public Camera Camera;
    public Selection Selection;

    private bool isClick = false;

    public bool IsShiftKeyDown = false;
    public bool IsAltKeyDown = false;

    public PointF MousePos = Point.Empty;
    public PointF MouseDownPos = Point.Empty;
    public PointF MouseUpPos = Point.Empty;

    public EditorInterface(Circuit circuit, Camera camera, Selection selection)
    {
        Circuit = circuit;
        Camera = camera;
        Selection = selection;  
    }

    public void MouseDown(PointF location, bool left)
    {
        MouseDownPos = MousePos;

        isClick = true;
        var obj = Circuit.GetAt(MousePos);

        if (left)
        {
            if (obj == null)
            {
                if (!IsShiftKeyDown)
                    Selection.ClearSelection();
                Selection.SelectAreaBegin(MousePos);
                isClick = false;
            }
            else if (obj.IsSelected)
            {

            }
            else
            {
                if (!IsShiftKeyDown)
                    Selection.ClearSelection();

                Selection.ToogleAt(MousePos);
                isClick = false;
            }


            //Circuit.CreateNode<OrGate>(MathF.Round(pos.X), MathF.Round(pos.Y));
        }
    }

    public void MouseMove(PointF location, bool left)
    {
        MousePos = Camera.ScreenToWorldSpace(location);

        if (left)
        {
            isClick = false;
            if (Selection.IsSelectingArea)
            {
                Selection.SelectAreaMove(MousePos);
            }
            else
            {
                Selection.Offset = new PointF(MousePos.X - MouseDownPos.X, MousePos.Y - MouseDownPos.Y);
            }
        }
        else
        {
            Selection.HoverAt(MousePos);
        }
    }

    public void MouseUp(PointF location, bool left)
    {
        MouseUpPos = MousePos;

        var obj = Circuit.GetAt(MousePos);

        if (isClick)
        {
            if (left)
            {
                if (obj == null)
                {

                }
                else if (obj.IsSelected)
                {
                    if (!IsShiftKeyDown)
                        Selection.ClearSelection();

                    Selection.ToogleAt(MousePos);
                }
                /*
                if (!isKeyDown(Keys.ShiftKey))
                {
                    Selection.ClearSelection();
                    Selection.ClickAt(pos);
                }
                */
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

    public void KeyDown(int keycode)
    {

    }
}


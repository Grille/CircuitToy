using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GGL.IO;

using CircuitLib;
using CircuitLib.Interface;

namespace CircuitLib_Tests;

public class MouseController
{
    Vector2 Position;
    bool LeftDown = false;
    CircuitEditor Editor;
    public MouseController(CircuitEditor editor)
    {
        Editor = editor;
    }

    public void Move(float x, float y)
    {
        Position = Editor.Camera.WorldToScreenSpace(new Vector2(x, y));
        Editor.MouseMove(new() { Location = Position, Left = LeftDown });
    }

    public void Down()
    {
        LeftDown = true;
        Editor.MouseDown(new() { Location = Position, Left = LeftDown });
    }

    public void Up()
    {
        Editor.MouseUp(new() { Location = Position, Left = LeftDown });
        LeftDown = false;
    }

    public void Click()
    {
        Down();
        Up();
    }
}

partial class Section
{
    public static void S07Interface()
    {
        TUtils.WriteTitle("TestInterface...");

        Tests.Run("Test Selection: ", () => {
            var circuit = new Circuit();
            var b0 = circuit.Nodes.Create<BufferGate>(new(2, 2));
            var b1 = circuit.Nodes.Create<BufferGate>(new(6, 4));

            var cam = new Camera();
            var editor = new CircuitEditor(circuit, cam);
            var mouse = new MouseController(editor);

            var hovered = editor.Selection.HoveredEntities;
            var selected = editor.Selection.SelectedEntities;

            mouse.Move(0, 0);
            mouse.Down();
            mouse.Move(10, 10);

            TUtils.AssertListContains(hovered, b0, b1);

            mouse.Up();

            TUtils.AssertListContains(selected, b0, b1);

            mouse.Move(0, 0);
            mouse.Click();

            TUtils.AssertValue(selected.Count, 0);

            mouse.Move(2, 2);

            TUtils.AssertListContains(hovered, b0);
            TUtils.AssertListContainsNot(hovered, b1);

            mouse.Click();

            TUtils.AssertListContains(selected, b0);
            TUtils.AssertListContainsNot(selected, b1);

            mouse.Move(0, 0);
            mouse.Click();

            editor.IsShiftKeyDown = true;

            mouse.Move(2, 2);
            mouse.Click();

            TUtils.AssertListContains(selected, b0);
            TUtils.AssertListContainsNot(selected, b1);

            mouse.Move(6, 4);
            mouse.Click();

            TUtils.AssertListContains(selected, b0, b1);

            mouse.Move(2, 2);
            mouse.Click();

            TUtils.AssertListContains(selected, b1);
            TUtils.AssertListContainsNot(selected, b0);

            TUtils.Success("OK");
        });


    }
}

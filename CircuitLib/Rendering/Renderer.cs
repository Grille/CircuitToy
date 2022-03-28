using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using CircuitLib.Math;
using CircuitLib.Interface;

namespace CircuitLib.Rendering;

public class Renderer
{
    IRendererBackend ctx;

    BoundingBox ViewPort;

    Camera camera;
    Theme theme;
    EditorInterface editor;
    Selection selection;

    Circuit circuit;
    PaintPalette palette;

    public Circuit Circuit {
        get {
            return circuit;
        }
        set {
            circuit = value;
        }
    }

    public Renderer(IRendererBackend renderBackend, Camera camera, Theme theme, EditorInterface @interface, Circuit circuit)
    {
        ctx = renderBackend;

        this.camera = camera;
        this.theme = theme;
        this.editor = @interface;
        this.selection = editor.Selection;
        this.circuit = circuit;

        palette = new PaintPalette();
    }

    public void Render()
    {
        var begin = camera.ScreenToWorldSpace(new Vector2(0, 0));
        var end = camera.ScreenToWorldSpace(new Vector2(camera.ScreenSize.Width, camera.ScreenSize.Height));

        ViewPort = new BoundingBox(begin, end);

        palette.Setup(ctx, theme, camera);

        ctx.Clear(palette.SceneBack);

        DrawGrid();


        if (selection.IsSelectingArea)
        {
            var pos = camera.WorldToScreenSpace(selection.SelectetArea.Begin);
            var size = selection.SelectetArea.Size * camera.Scale;

            ctx.FillRectangle(palette.SelectionTransparent, pos, size);
            ctx.DrawRectangle(palette.SelectionOpaque, pos, size);
        }


        DrawSelection();

        foreach (var net in circuit.Networks)
        {
            DrawNetwork(net);
        }

        foreach (var node in circuit.Nodes)
        {
            DrawNode(node);
        }



        if (selection.HoveredEntity != null)
        {
            var entity = selection.HoveredEntity;
            var bounds = entity.Bounds;

            var pos = camera.WorldToScreenSpace(bounds.Begin);
            var size = bounds.Size * camera.Scale;


            ctx.DrawRectangle(palette.SelectionHover, pos, size);
        }

    }




    public void DrawNetwork(Network network)
    {
        int paint = network.State switch {
            State.Off => palette.StateOff,
            State.Low => palette.StateLow,
            State.High => palette.StateHigh,
            _ => palette.StateError,
        };

        foreach (var wire in network.Wires)
        {
            var pos0 = camera.WorldToScreenSpace(wire.StartPin.Position);
            var pos1 = camera.WorldToScreenSpace(wire.EndPin.Position);

            ctx.DrawLine(paint, pos0, pos1);
        }

        foreach (var pin in network.GuardPins)
        {
            var pos = camera.WorldToScreenSpace(pin.Position);

            float rad = pin.ConnectedWires.Count == 2 ?
                Theme.NetPinInlineRadius * camera.Scale :
                Theme.NetPinJointRadius * camera.Scale;

            ctx.FillCircle(paint, pos, rad);

        }
    }

    public void DrawNode(Node node)
    {
        if (!node.IsVisible || (node.IsSelected && editor.IsMoving))
            return;

        foreach (var pin in node.InputPins)
        {
            var pos = camera.WorldToScreenSpace(pin.Position);
            var rad = Theme.IoPinRadius * camera.Scale;

            int paint = pin.State switch {
                State.Off => palette.StateOff,
                State.Low => palette.StateLow,
                State.High => palette.StateHigh,
                _ => palette.StateError,
            };

            ctx.FillCircle(paint, pos, rad);
        }

        foreach (var pin in node.OutputPins)
        {
            var pos = camera.WorldToScreenSpace(pin.Position);
            var rad = Theme.IoPinRadius * camera.Scale;

            int paint = pin.State switch {
                State.Off => palette.StateOff,
                State.Low => palette.StateLow,
                State.High => palette.StateHigh,
                _ => palette.StateError,
            };

            ctx.FillCircle(paint, pos, rad);
        }

        var screenPos = camera.WorldToScreenSpace(node.Position);
        var screenSize = node.Size * camera.Scale;
        var scrennHalfSize = screenSize * 0.5f;
        var drawPos = screenPos - scrennHalfSize;

        ctx.FillRectangle(palette.NodeBack, drawPos, screenSize);
        ctx.DrawText(palette.NodeText, node.DisplayName, drawPos, screenSize);
    }

    public void DrawGrid(float gridSize, int paint)
    {
        float scaledGridSize = gridSize * camera.Scale;

        if (scaledGridSize < 5)
            return;

        var clientSize = camera.ScreenSize;

        var distToNull = camera.WorldToScreenSpace(Vector2.Zero);
        float offsetX = distToNull.X % scaledGridSize;
        float offsetY = distToNull.Y % scaledGridSize;

        int countX = (int)(clientSize.Width / scaledGridSize);
        int countY = (int)(clientSize.Height / scaledGridSize);

        for (int ix = 0; ix <= countX; ix++)
        {
            int posX = (int)((ix * scaledGridSize) + offsetX);
            ctx.DrawLine(paint, new(posX, 0), new(posX, clientSize.Height));
        }

        for (int iy = 0; iy <= countY; iy++)
        {
            int posY = (int)((iy * scaledGridSize) + offsetY);
            ctx.DrawLine(paint, new(0, posY), new(clientSize.Width, posY));
        }
    }

    public void DrawGrid()
    {
        DrawGrid(1, palette.SceneGrid);
        DrawGrid(10, palette.SceneGrid);
        DrawGrid(100, palette.SceneGrid);
    }

    public void DrawSelection()
    {
        var outlineOffset = new Vector2(Theme.SelectionOutline, Theme.SelectionOutline);

        foreach (var entity in selection.SelectedEntities)
        {
            if (entity is Node)
            {
                var node = (Node)entity;

                foreach (var pin in node.InputPins)
                {
                    drawSelectedPin(pin);
                }

                foreach (var pin in node.OutputPins)
                {
                    drawSelectedPin(pin);
                }

                var screenPos = camera.WorldToScreenSpace(node.Position + selection.SnapOffset);
                var screenSize = node.Size * camera.Scale;
                var scrennHalfSize = screenSize * 0.5f;
                var drawPos = screenPos - scrennHalfSize;

                ctx.DrawRectangle(palette.SelectionOutline, drawPos, screenSize);
                ctx.FillRectangle(palette.SelectionOpaque, drawPos, screenSize);
                //ctx.DrawText(palette.NodeText, node.DisplayName, drawPos, screenSize);
            }
            if (entity is NetPin)
            {
                var pin = (NetPin)entity;

                drawSelectedPin(pin);
            }
        }

        void drawSelectedPin(Pin pin)
        {
            var pos = camera.WorldToScreenSpace(pin.Position + selection.SnapOffset);
            float rad = 0f;

            if (pin is NetPin)
            {
                if (pin.ConnectedWires.Count == 2)
                {
                    rad = (Theme.NetPinInlineRadius + Theme.SelectionOutline) * camera.Scale;
                }
                else
                {
                    rad = (Theme.NetPinJointRadius + Theme.SelectionOutline) * camera.Scale;
                }
            }
            else if (pin is IOPin)
            {
                rad = (Theme.IoPinRadius + Theme.SelectionOutline) * camera.Scale;
            }

            ctx.FillCircle(palette.SelectionOpaque, pos, rad);

            foreach (var wire in pin.ConnectedWires)
            {
                var pin2 = wire.GetOtherPin(pin);

                Vector2 pos2;

                if (selection.SelectedEntities.Contains(pin2))
                    pos2 = camera.WorldToScreenSpace(pin2.Position + selection.SnapOffset);
                else
                    pos2 = camera.WorldToScreenSpace(pin2.Position);

                ctx.DrawLine(palette.SelectionWire, pos, pos2);
            }
        };
    }
}

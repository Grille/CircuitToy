using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using CircuitLib.IntMath;
using CircuitLib.Interface;
using System.Diagnostics;
using CircuitLib.Interface.UserTools;

namespace CircuitLib.Rendering;

public class Renderer<TRenderer> where TRenderer : IRendererBackend
{
    TRenderer ctx;

    BoundingBox ViewPort;

    Camera camera;
    Theme theme;
    CircuitEditor editor;
    Selection selection;

    Circuit circuit;
    PaintPalette palette;

    public int StatsEntityDrawCount;

    public Circuit Circuit {
        get {
            return circuit;
        }
        set {
            circuit = value;
        }
    }

    public Renderer(TRenderer renderBackend, Theme theme, CircuitEditor editor)
    {
        ctx = renderBackend;

        camera = editor.Camera;
        this.theme = theme;
        this.editor = editor;
        circuit = editor.Circuit;
        selection = editor.Selection;

        palette = new PaintPalette();
    }

    public void Render()
    {
        StatsEntityDrawCount = 0;

        var sw = new Stopwatch();
        sw.Start();

        var begin = camera.ScreenToWorldSpace(Vector2.Zero);
        var end = camera.ScreenToWorldSpace(camera.ScreenSize);

        ViewPort = new BoundingBox(begin, end);

        palette.Setup(ctx, theme, camera);

        ctx.Clear(palette.SceneBack);

        DrawGrid();

        DrawHoverd();
        DrawSelection();

        if (circuit != null)
        {
            foreach (var net in circuit.Networks)
            {
                if (ViewPort.IsColliding(net.Bounds))
                {
                    DrawNetwork(net);
                }
            }

            foreach (var node in circuit.Nodes)
            {
                if (ViewPort.IsColliding(node.Bounds))
                {
                    DrawNode(node);
                }
            }
        }

        if (selection.IsSelectingArea)
        {
            var pos = camera.WorldToScreenSpace(selection.SelectetArea.Begin);
            var size = selection.SelectetArea.Size * camera.Scale;

            ctx.FillRectangle(palette.SelectionTransparent, pos, size);
            ctx.DrawRectangle(palette.SelectionOpaque, pos, size);
        }

        if (selection.HoveredEntities.Count == 1)
        {
            var entity = selection.HoveredEntities[0];
            var str = entity.GetDebugStr();
            var size = ctx.MeasureText(palette.DebugText, str);
            var pos = editor.ScreenMousePos + new Vector2(10, 0);
            ctx.FillRectangle(palette.DebugBackTransparent, pos, size);
            ctx.DrawText(palette.DebugText, entity.GetDebugStr(), pos);
        }

        {
            var pos = new Vector2(10, 10);
            var sb = new StringBuilder();
            sb.AppendLine($"Delta: {sw.Elapsed.TotalMilliseconds}ms");
            sb.AppendLine($"EntityCount: {StatsEntityDrawCount}");
            string info = sb.ToString();


            var size = ctx.MeasureText(palette.DebugText, info);
            ctx.FillRectangle(palette.DebugBackTransparent, pos, size);
            ctx.DrawText(palette.DebugText, info, pos);
        }

        if (editor.UserTool is UserToolAddWire) {
            var tool = editor.UserTool as UserToolAddWire;
            if (tool.Wireing)
            {
                int paint = tool.Error ? palette.SelectionErrorWire : palette.HoverdWire;
                DrawWireSilhouette(editor.WorldMouseDownPos.Round(), editor.WorldMousePos.Round(), paint);
            }
        }

        //ctx.DrawLine(palette.StateError, new(100, 100), new(100, 250));
        //ctx.FillPolygon(palette.StateError, new Vector2[] { new(100, 100), new(100, 200), new(200, 200) });

        ctx.Cleanup();
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
            var worldPos0 = wire.StartPin.Position;
            if (wire.StartPin.IsSelected)
                worldPos0 += selection.SnapOffset;

            var worldPos1 = wire.EndPin.Position;
            if (wire.EndPin.IsSelected)
                worldPos1 += selection.SnapOffset;

            var pos0 = camera.WorldToScreenSpace(worldPos0);
            var pos1 = camera.WorldToScreenSpace(worldPos1);

            ctx.DrawLine(paint, pos0, pos1);

            StatsEntityDrawCount++;
        }

        foreach (var pin in network.Pins.NetPins)
        {
            var worldPos = pin.Position;
            if (pin.IsSelected)
                worldPos += selection.SnapOffset;

            var pos = camera.WorldToScreenSpace(worldPos);

            float rad = pin.ConnectedWires.Count == 2 ?
                Theme.NetPinInlineRadius * camera.Scale :
                Theme.NetPinJointRadius * camera.Scale;

            ctx.FillCircle(paint, pos, rad);

            StatsEntityDrawCount++;
        }

        StatsEntityDrawCount++;
    }

    public void DrawNode(Node node)
    {
        var offset = Vector2.Zero;
        if (node.IsSelected)
            offset += selection.SnapOffset;

        foreach (var pin in node.InputPins)
        {
            var pos = camera.WorldToScreenSpace(pin.Position + offset);
            var rad = Theme.IoPinRadius * camera.Scale;

            int paint = pin.State switch {
                State.Off => palette.StateOff,
                State.Low => palette.StateLow,
                State.High => palette.StateHigh,
                _ => palette.StateError,
            };

            ctx.FillCircle(paint, pos, rad);

            StatsEntityDrawCount++;
        }

        foreach (var pin in node.OutputPins)
        {
            var pos = camera.WorldToScreenSpace(pin.Position + offset);
            var rad = Theme.IoPinRadius * camera.Scale;

            int paint = pin.State switch {
                State.Off => palette.StateOff,
                State.Low => palette.StateLow,
                State.High => palette.StateHigh,
                _ => palette.StateError,
            };

            ctx.FillCircle(paint, pos, rad);

            StatsEntityDrawCount++;
        }

        var screenPos = camera.WorldToScreenSpace(node.Position + offset);
        var screenSize = node.Size * camera.Scale;
        var scrennHalfSize = screenSize * 0.5f;
        var drawPos = screenPos - scrennHalfSize;

        ctx.FillRectangle(palette.NodeBack, drawPos, screenSize);
        ctx.DrawRectangle(palette.NodeBorder, drawPos, screenSize);
        ctx.DrawText(palette.NodeText, node.DisplayName, drawPos, screenSize);

        StatsEntityDrawCount++;
    }

    public void DrawGrid(float gridSize, int paint)
    {
        float scaledGridSize = gridSize * camera.Scale;

        if (scaledGridSize < 20)
            return;

        var clientSize = camera.ScreenSize;

        var distToNull = camera.WorldToScreenSpace(Vector2.Zero);
        float offsetX = distToNull.X % scaledGridSize;
        float offsetY = distToNull.Y % scaledGridSize;

        int countX = (int)(clientSize.X / scaledGridSize);
        int countY = (int)(clientSize.Y / scaledGridSize);

        for (int ix = 0; ix <= countX; ix++)
        {
            int posX = (int)((ix * scaledGridSize) + offsetX);
            ctx.DrawLine(paint, new(posX, 0), new(posX, clientSize.Y));
        }

        for (int iy = 0; iy <= countY; iy++)
        {
            int posY = (int)((iy * scaledGridSize) + offsetY);
            ctx.DrawLine(paint, new(0, posY), new(clientSize.X, posY));
        }
    }

    public void DrawGrid()
    {
        DrawGrid(1, palette.SceneGrid);
        //DrawGrid(10, palette.SceneGrid);
        //DrawGrid(100, palette.SceneGrid);
    }

    public void DrawNodeSilhouette(Node node, int fillPaint, int outlinePaint)
    {
        var offset = Vector2.Zero;
        if (node.IsSelected)
            offset = selection.SnapOffset;

        foreach (var pin in node.InputPins)
        {
            DrawPinSilhouette(pin, fillPaint);
        }
        foreach (var pin in node.OutputPins)
        {
            DrawPinSilhouette(pin, fillPaint);
        }

        var screenPos = camera.WorldToScreenSpace(node.Position + offset);
        var screenSize = node.Size * camera.Scale;
        var scrennHalfSize = screenSize * 0.5f;
        var drawPos = screenPos - scrennHalfSize;

        ctx.DrawRectangle(outlinePaint, drawPos, screenSize);
    }

    public void DrawPinSilhouette(Pin pin, int fillPaint)
    {
        var offset = Vector2.Zero;
        if (pin.IsSelected)
            offset = selection.SnapOffset;

        var pos = camera.WorldToScreenSpace(pin.Position + offset);
        float rad = 0f;

        if (pin is WirePin)
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

        ctx.FillCircle(fillPaint, pos, rad);
    }

    public void DrawWireSilhouette(Vector2 worldPos0, Vector2 worldPos1, int wirePaint)
    {
        var pos0 = camera.WorldToScreenSpace(worldPos0);
        var pos1 = camera.WorldToScreenSpace(worldPos1);

        ctx.DrawLine(wirePaint, pos0, pos1);
    }
    public void DrawWireSilhouette(Wire wire, int wirePaint)
    {
        var worldPos0 = wire.StartPin.Position;
        if (wire.StartPin.IsSelected)
            worldPos0 += selection.SnapOffset;

        var worldPos1 = wire.EndPin.Position;
        if (wire.EndPin.IsSelected)
            worldPos1 += selection.SnapOffset;

        var pos0 = camera.WorldToScreenSpace(worldPos0);
        var pos1 = camera.WorldToScreenSpace(worldPos1);

        ctx.DrawLine(wirePaint, pos0, pos1);
    }


    public void DrawSilhouette(Entity entity, int fillPaint, int outlinePaint, int wirePaint)
    {
        if (entity is Node)
        {
            var node = (Node)entity;

            DrawNodeSilhouette(node,fillPaint,outlinePaint);
            return;
        }
        if (entity is Pin)
        {
            var pin = (Pin)entity;

            DrawPinSilhouette(pin,fillPaint);
            return;
        }
        if (entity is Wire)
        {
            var wire = (Wire)entity;

            DrawWireSilhouette(wire,wirePaint);
            return;
        }
    }

    public void DrawSilhouetteList(IList<Entity> entitys, int fillPaint, int outlinePaint, int wirePaint)
    {
        foreach (var entity in entitys)
        {
            DrawSilhouette(entity, fillPaint, outlinePaint, wirePaint);
        }
    }

    public void DrawSelection()
    {
        DrawSilhouetteList(selection.SelectedEntities, palette.SelectionOpaque, palette.SelectionOutline, palette.SelectionOutlineWire);
    }

    public void DrawHoverd()
    {

        DrawSilhouetteList(selection.HoveredEntities, palette.HoverdOpaque, palette.HoverdOutline, palette.HoverdOutlineWire);
    }
}

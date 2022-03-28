using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using CircuitLib.Interface;

namespace CircuitLib.Rendering;

internal class PaintPalette
{
    IRendererBackend ctx;

    public int SceneBack;
    public int SceneGrid;

    public int NodeBack;
    public int NodeText;

    public int StateLow;
    public int StateHigh;
    public int StateOff;
    public int StateError;

    public int SelectionOpaque;
    public int SelectionTransparent;
    public int SelectionWire;
    public int SelectionOutline;
    public int SelectionOutlineWire;

    public int SelectionHover;

    public int Debug;

    public void Setup(IRendererBackend ctx, Theme theme, Camera cam)
    {
        this.ctx = ctx;

        float scaledWireWidth = Theme.WireWidth * cam.Scale;

        SceneBack = ctx.CreatePaint(theme.SceneBackColor);
        SceneGrid = ctx.CreatePaint(theme.SceneGridColor);

        NodeBack = ctx.CreatePaint(theme.NodeBackColor);
        NodeText = ctx.CreateStrPaint(theme.NodeTextColor, 1 * cam.Scale, "consolas", Theme.NodeTextSize * cam.Scale);

        StateLow = ctx.CreatePaint(theme.StateLowColor, scaledWireWidth);
        StateHigh = ctx.CreatePaint(theme.StateHighColor, scaledWireWidth);
        StateOff = ctx.CreatePaint(theme.StateOffColor, scaledWireWidth);
        StateError = ctx.CreatePaint(theme.StateErrorColor, scaledWireWidth);

        SelectionOpaque = ctx.CreatePaint(theme.SelectionColor,2);
        SelectionTransparent = ctx.CreatePaint(Color.FromArgb(50, theme.SelectionColor));

        SelectionWire = ctx.CreatePaint(theme.SelectionColor, (Theme.WireWidth) * cam.Scale);
        SelectionOutline = ctx.CreatePaint(theme.SelectionColor, (Theme.SelectionOutline * 2) * cam.Scale);
        SelectionOutlineWire = ctx.CreatePaint(theme.SelectionColor, (Theme.WireWidth + Theme.SelectionOutline * 2) * cam.Scale);
        SelectionHover = ctx.CreatePaint(theme.HoverColor,2);

        Debug = ctx.CreatePaint(theme.DebugColor);
    }

    public void Cleanup()
    {
        ctx.DestroyPaint(SceneBack);
        ctx.DestroyPaint(SceneGrid);

        ctx.DestroyPaint(NodeBack);
        ctx.DestroyPaint(NodeText);

        ctx.DestroyPaint(StateLow);
        ctx.DestroyPaint(StateHigh);
        ctx.DestroyPaint(StateOff);
        ctx.DestroyPaint(StateError);

        ctx.DestroyPaint(SelectionOpaque);
        ctx.DestroyPaint(SelectionTransparent);
        ctx.DestroyPaint(SelectionWire);
        ctx.DestroyPaint(SelectionOutline);
        ctx.DestroyPaint(SelectionOutlineWire);
        ctx.DestroyPaint(SelectionHover);

        ctx.DestroyPaint(Debug);

    }
}

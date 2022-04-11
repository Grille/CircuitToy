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
    public int NodeBorder;
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

    public int HoverdOpaque;
    public int HoverdTransparent;
    public int HoverdWire;
    public int HoverdOutline;
    public int HoverdOutlineWire;

    public int Debug;
    public int DebugText;
    public int DebugBackTransparent;

    public void Setup(IRendererBackend ctx, Theme theme, Camera cam)
    {
        this.ctx = ctx;

        float scaledWireWidth = Theme.WireWidth * cam.Scale;

        SceneBack = ctx.CreatePaint(theme.SceneBackColor);
        SceneGrid = ctx.CreatePaint(theme.SceneGridColor);

        NodeBack = ctx.CreatePaint(theme.NodeBackColor);
        NodeBorder = ctx.CreatePaint(theme.NodeBorderColor, Theme.WireWidth * cam.Scale);
        NodeText = ctx.CreateFontPaint(theme.NodeTextColor, 1 * cam.Scale, "consolas", Theme.NodeTextSize * cam.Scale);

        StateLow = ctx.CreatePaint(theme.StateLowColor, scaledWireWidth);
        StateHigh = ctx.CreatePaint(theme.StateHighColor, scaledWireWidth);
        StateOff = ctx.CreatePaint(theme.StateOffColor, scaledWireWidth);
        StateError = ctx.CreatePaint(theme.StateErrorColor, scaledWireWidth);

        SelectionOpaque = ctx.CreatePaint(theme.SelectionColor,2);
        SelectionTransparent = ctx.CreatePaint(Color.FromArgb(50, theme.SelectionColor));
        SelectionWire = ctx.CreatePaint(theme.SelectionColor, (Theme.WireWidth) * cam.Scale);
        SelectionOutline = ctx.CreatePaint(theme.SelectionColor, (Theme.SelectionOutline * 2) * cam.Scale);
        SelectionOutlineWire = ctx.CreatePaint(theme.SelectionColor, (Theme.WireWidth + Theme.SelectionOutline * 2) * cam.Scale);

        HoverdOpaque = ctx.CreatePaint(theme.HoverColor,2);
        HoverdTransparent = ctx.CreatePaint(Color.FromArgb(50, theme.HoverColor));
        HoverdWire = ctx.CreatePaint(theme.HoverColor, (Theme.WireWidth) * cam.Scale);
        HoverdOutline = ctx.CreatePaint(theme.HoverColor, (Theme.SelectionOutline * 2) * cam.Scale);
        HoverdOutlineWire = ctx.CreatePaint(theme.HoverColor, (Theme.WireWidth + Theme.SelectionOutline * 2) * cam.Scale);

        Debug = ctx.CreatePaint(theme.DebugColor);
        DebugText = ctx.CreateFontPaint(theme.DebugColor, 1, "consolas", 12);
        DebugBackTransparent = ctx.CreatePaint(Color.FromArgb(150, 0, 0, 0), 1);
    }
}

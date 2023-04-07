using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Rendering;

public class Theme
{
    public string Name = "Theme";

    public const float IoPinRadius = 0.5f;
    public const float NetPinInlineRadius = 0.1f;
    public const float NetPinJointRadius = 0.2f;
    public const float WireWidth = 0.2f;
    public const float NodeTextSize = 1f;
    public const float SelectionOutline = 0.2f;

    public Color SceneBackColor;
    public Color SceneGridColor;

    public Color NodeBackColor;
    public Color NodeBorderColor;
    public Color NodeTextColor;

    public Color StateLowColor;
    public Color StateHighColor;
    public Color StateOffColor;
    public Color StateErrorColor;

    public Color SelectionColor;
    public Color SelectionErrorColor;
    public Color HoverColor;
    public Color DebugColor;

    public readonly static Theme Dark = new() {
        SceneBackColor = Color.FromArgb(0, 0, 0),
        SceneGridColor = Color.FromArgb(20, 20, 20),

        NodeBorderColor = Color.FromArgb(10, 30, 30),
        NodeBackColor = Color.FromArgb(20, 50, 50),
        NodeTextColor = Color.LightGray,

        StateLowColor = Color.DarkGray,
        StateHighColor = Color.DarkCyan,
        StateOffColor = Color.FromArgb(60, 80, 80),
        StateErrorColor = Color.DarkMagenta,

        SelectionColor = Color.Green,
        SelectionErrorColor = Color.DarkRed,
        HoverColor = Color.LimeGreen,
        DebugColor = Color.Lime,
    };

    public readonly static Theme Light = new() {
        SceneBackColor = Color.White,
        SceneGridColor = Color.WhiteSmoke,

        NodeBorderColor = Color.DarkGray,
        NodeBackColor = Color.LightGray,
        NodeTextColor = Color.Black,

        StateLowColor = Color.Black,
        StateHighColor = Color.Blue,
        StateOffColor = Color.FromArgb(150, 150, 150),
        StateErrorColor = Color.Red,

        SelectionColor = Color.Green,
        SelectionErrorColor = Color.DarkRed,
        HoverColor = Color.LimeGreen,
        DebugColor = Color.Lime,
    };

    public readonly static Theme Contrast = new() {
        SceneBackColor = Color.FromArgb(0, 0, 0),
        SceneGridColor = Color.FromArgb(20, 20, 20),

        NodeBorderColor = Color.FromArgb(80, 80, 80),
        NodeBackColor = Color.FromArgb(0, 0, 0),
        NodeTextColor = Color.LightGray,

        StateLowColor = Color.DarkGray,
        StateHighColor = Color.Lime,
        StateOffColor = Color.FromArgb(80, 80, 80),
        StateErrorColor = Color.Red,

        SelectionColor = Color.Green,
        SelectionErrorColor = Color.DarkRed,
        HoverColor = Color.LimeGreen,
        DebugColor = Color.Lime,
    };
}
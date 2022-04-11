using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Rendering;

public record class Theme
{
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
    public Color HoverColor;
    public Color DebugColor;
}
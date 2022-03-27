using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CircuitLib.Rendering;

public record class Theme
{
    public float IoPinRadius = 0.5f;
    public float NetPinInlineRadius = 0.1f;
    public float NetPinJointRadius = 0.2f;
    public float WireWidth = 0.2f;
    public float NodeTextSize = 1f;

    public Color SceneBackColor;
    public Color SceneGridColor;

    public Color NodeBackColor;
    public Color NodeTextColor;

    public Color StateLowColor;
    public Color StateHighColor;
    public Color StateOffColor;
    public Color StateErrorColor;

    public Color SelectionColor;
    public Color HoverColor;
    public Color DebugColor;
}

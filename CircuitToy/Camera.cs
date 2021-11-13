using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CircuitToy;

class Camera
{
    public float PosX = 0, PosY = 0;
    public float Scale = 10;

    private float width, height;
    private float hWidth, hHeight;

    private PointF lastLocation;

    public PointF Position
    {
        get => new PointF(PosX, PosY);
        set
        {
            PosX = value.X;
            PosY = value.Y;
        }
    }

    public Size ScreenSize
    {
        get => new Size((int)width, (int)height);
        set
        {
            width = value.Width;
            height = value.Height;
            hWidth = width / 2;
            hHeight = height / 2;
        }
    }

    public void MouseScrollEvent(MouseEventArgs e, float scrollFactor)
    {
        var oldWorldPos = ScreenToWorldSpace(e.Location);
        if (e.Delta > 0)
            Scale *= scrollFactor;
        else
            Scale /= scrollFactor;

        var newWorldPos = ScreenToWorldSpace(e.Location);
        PosX += oldWorldPos.X - newWorldPos.X;
        PosY += oldWorldPos.Y - newWorldPos.Y;
    }

    public void MouseMoveEvent(MouseEventArgs e, bool move)
    {
        if (move)
        {
            var oldWorldPos = ScreenToWorldSpace(lastLocation);
            var newWorldPos = ScreenToWorldSpace(e.Location);
            PosX += oldWorldPos.X - newWorldPos.X;
            PosY += oldWorldPos.Y - newWorldPos.Y;
        }
        lastLocation = e.Location;
    }

    public PointF ScreenToWorldSpace(PointF screenPos)
    {
        var transpos = new PointF((screenPos.X - hWidth) / Scale, (screenPos.Y - hHeight) / Scale);
        var pos = new PointF(transpos.X + PosX, transpos.Y + PosY);
        return pos;
    }

    public PointF WorldToScreenSpace(PointF worldPos)
    {
        var transpos = new PointF(worldPos.X - PosX, worldPos.Y - PosY);
        var pos = new PointF(transpos.X * Scale + hWidth, transpos.Y * Scale + hHeight);
        return pos;
    }
}


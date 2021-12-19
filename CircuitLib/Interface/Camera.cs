using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CircuitLib.Interface;

public class Camera
{
    public float PosX = 0, PosY = 0;
    public float Scale = 10;

    private float width, height;
    private float hWidth, hHeight;

    private Vector2 lastLocation;

    public Vector2 Position
    {
        get => new Vector2(PosX, PosY);
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

    public void MouseScrollEvent(Vector2 location, float delta, float scrollFactor)
    {
        var oldWorldPos = ScreenToWorldSpace(location);
        if (delta > 0)
            Scale *= scrollFactor;
        else
            Scale /= scrollFactor;

        var newWorldPos = ScreenToWorldSpace(location);
        PosX += oldWorldPos.X - newWorldPos.X;
        PosY += oldWorldPos.Y - newWorldPos.Y;
    }

    public void MouseMoveEvent(Vector2 location, bool move)
    {
        if (move)
        {
            var oldWorldPos = ScreenToWorldSpace(lastLocation);
            var newWorldPos = ScreenToWorldSpace(location);
            PosX += oldWorldPos.X - newWorldPos.X;
            PosY += oldWorldPos.Y - newWorldPos.Y;
        }
        lastLocation = location;
    }

    public Vector2 ScreenToWorldSpace(Vector2 screenPos)
    {
        var transpos = new Vector2((screenPos.X - hWidth) / Scale, (screenPos.Y - hHeight) / Scale);
        var pos = new Vector2(transpos.X + PosX, transpos.Y + PosY);
        return pos;
    }

    public Vector2 WorldToScreenSpace(Vector2 worldPos)
    {
        var transpos = new Vector2(worldPos.X - PosX, worldPos.Y - PosY);
        var pos = new Vector2(transpos.X * Scale + hWidth, transpos.Y * Scale + hHeight);
        return pos;
    }
}


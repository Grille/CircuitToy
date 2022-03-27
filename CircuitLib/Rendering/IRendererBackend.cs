﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Numerics;

namespace CircuitLib.Rendering;

public interface IRendererBackend
{
    public void Clear(int paint);

    public void DrawRectangle(int paint, Vector2 position, Vector2 size);
    public void FillRectangle(int paint, Vector2 position, Vector2 size);

    public void DrawLine(int paint, Vector2 position1, Vector2 position2);

    public void DrawCircle(int paint, Vector2 position, float radius);

    public void FillCircle(int paint, Vector2 position, float radius);

    public void DrawText(int paint, string text, Vector2 position);

    public void DrawText(int paint, string text, Vector2 position, Vector2 size);

    public void DrawText(int paint, string text, RectangleF rect);

    public int CreatePaint();

    public int CreatePaint(Color color);

    public int CreatePaint(Color color, float width);

    public int CreateStrPaint(Color color, float width, string font, float size);

    public void DestroyPaint(int id);

}
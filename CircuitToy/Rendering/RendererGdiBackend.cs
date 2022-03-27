using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using CircuitLib.Rendering;

namespace CircuitToy;

internal class RendererGdiBackend : IRendererBackend
{
    List<GdiPaint> Paints;
    Graphics g;

    public RendererGdiBackend() 
    {
        Paints = new List<GdiPaint>();
    }

    public void UseGraphics(Graphics graphics)
    {
        g = graphics;
    }

    public void Clear(int paint)
    {
        g.Clear(Paints[paint].Color);
    }

    public void DrawRectangle(int paint, Vector2 position, Vector2 size)
    {
        g.DrawRectangle(Paints[paint].Pen, position.X, position.Y, size.X, size.Y);
    }

    public void FillRectangle(int paint, Vector2 position, Vector2 size)
    {
        g.FillRectangle(Paints[paint].Brush, position.X, position.Y, size.X, size.Y);
    }

    public void DrawLine(int paint, Vector2 position1, Vector2 position2)
    {
        g.DrawLine(Paints[paint].Pen, position1.X, position1.Y, position2.X, position2.Y);
    }

    public void DrawCircle(int paint, Vector2 position, float radius)
    {
        g.DrawEllipse(Paints[paint].Pen, position.X - radius, position.Y - radius, radius * 2, radius * 2);
    }

    public void FillCircle(int paint, Vector2 position, float radius)
    {
        g.FillEllipse(Paints[paint].Brush, position.X - radius, position.Y - radius, radius * 2, radius * 2);
    }

    public void DrawText(int paint, string text, Vector2 position)
    {
        var p = Paints[paint];
        g.DrawString(text, p.Font, p.Brush, position.X, position.Y);
    }

    public void DrawText(int paint, string text, Vector2 position, Vector2 size)
    {
        DrawText(paint, text, new RectangleF(position.X, position.Y, size.X, size.Y));
    }

    public void DrawText(int paint, string text, RectangleF rect)
    {
        var p = Paints[paint];

        var format = new StringFormat(StringFormatFlags.NoWrap) {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        g.DrawString(text, p.Font, p.Brush, rect, format);
    }

    public int CreatePaint()
    {
        return CreatePaint(Color.Black, 1);
    }

    public int CreatePaint(Color color)
    {
        return CreatePaint(color, 1);
    }

    public int CreatePaint(Color color, float width)
    {
        var paint = new GdiPaint();
        paint.Color = color;
        paint.Brush = new SolidBrush(color);
        paint.Pen = new Pen(color, width);

        Paints.Add(paint);

        return Paints.IndexOf(paint);
    }

    public int CreateStrPaint(Color color, float width, string font, float size)
    {
        var paint = new GdiPaint();
        paint.Color = color;
        paint.Brush = new SolidBrush(color);
        paint.Pen = new Pen(color, size);
        paint.Font = new Font(font, size);


        Paints.Add(paint);

        return Paints.IndexOf(paint);
    }

    public void DestroyPaint(int id)
    {
        var paint = Paints[id];
        paint.Brush?.Dispose();
        paint.Pen?.Dispose();
        paint.Font?.Dispose();

        Paints.Remove(paint);
    }
}

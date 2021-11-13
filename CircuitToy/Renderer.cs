using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CircuitLib;

namespace CircuitToy;

internal class Renderer
{
    Camera camera;
    Control target;
    Graphics? g;
    public Renderer(Control target, Camera camera)
    {
        this.target = target;
        this.camera = camera;
    }

    public void Render(Graphics g, Circuit circuit)
    {
        camera.ScreenSize = target.ClientSize;
        this.g = g;
        g.Clear(Color.White);
        drawGrid();

        foreach (var node in circuit.Nodes)
        {
            var rect = new RectangleF(node.Position.X-1.5f, node.Position.Y-1.5f, 3, 3);
            DrawRectangle(new Pen(Brushes.Black, 0.1f), rect);
            drawString(node.Name, new Font("consolas", 0.5f), Brushes.Black, rect);
        }


        g.Flush();
    }

    void drawGrid()
    {
        drawGrid(1, Pens.WhiteSmoke);
        drawGrid(10, Pens.WhiteSmoke);
        drawGrid(100, Pens.WhiteSmoke);
        drawCircle(PointF.Empty, 1, Pens.Red);
    }
    void drawGrid(float gridSize, Pen pen)
    {
        float scaledGridSize = gridSize * camera.Scale;

        if (scaledGridSize < 5)
            return;

        var clientSize = target.ClientSize;

        var distToNull = camera.WorldToScreenSpace(PointF.Empty);
        float offsetX = distToNull.X % scaledGridSize;
        float offsetY = distToNull.Y % scaledGridSize;

        int countX = (int)(clientSize.Width / scaledGridSize);
        int countY = (int)(clientSize.Height / scaledGridSize);

        for (int ix = 0; ix <= countX; ix++)
        {
            int posX = (int)((ix * scaledGridSize) + offsetX);
            g.DrawLine(pen, new(posX, 0), new(posX, clientSize.Height));
        }

        for (int iy = 0; iy <= countY; iy++)
        {
            int posY = (int)((iy * scaledGridSize) + offsetY);
            g.DrawLine(pen, new(0, posY), new(clientSize.Width, posY));
        }
    }
    void drawCircle(PointF pos,float radius, Pen pen)
    {
        var drawPos = camera.WorldToScreenSpace(pos);
        int scaledRadius = (int)(radius * camera.Scale);

        g.DrawEllipse(pen, new((int)drawPos.X - scaledRadius, (int)drawPos.Y - scaledRadius, scaledRadius * 2, scaledRadius * 2));
    }

    void DrawRectangle(Pen pen,RectangleF rect)
    {
        var npen = new Pen(pen.Color, pen.Width*camera.Scale);
        var drawPos = camera.WorldToScreenSpace(rect.Location);
        float width = rect.Width * camera.Scale;
        float height = rect.Height * camera.Scale;
        g.DrawRectangle(npen, drawPos.X, drawPos.Y, width, height);
    }

    void drawString(string text, Font font, Brush brush, RectangleF rect)
    {
        var nfont = new Font(font.Name,font.Size * camera.Scale);
        var drawPos = camera.WorldToScreenSpace(rect.Location);
        float width = rect.Width * camera.Scale;
        float height = rect.Height * camera.Scale;
        var format = new StringFormat(StringFormatFlags.NoWrap  ) { 
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };
        g.DrawString(text, nfont, brush, new RectangleF(drawPos.X, drawPos.Y, width, height), format);
    }


}


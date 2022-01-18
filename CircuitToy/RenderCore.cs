using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Windows.Forms;
using CircuitLib.Math;
using CircuitLib.Interface;

namespace CircuitToy
{
    enum ColorB {
        True,
        False,
        Heigh,
        Error,
    }

    internal class RenderCore
    {
        Control target;
        Camera camera;
        Graphics g;
        public BoundingBox ViewPort {
            private set; 
            get;
        } 

        bool _highQuality = true;
        public bool HighQuality {
            set { 
                _highQuality = value;

                updateQuality();
            }
            get { 
                return _highQuality; 
            }
        }

        public RenderCore(Control target, Camera camera)
        {
            this.target = target;
            this.camera = camera;
        }

        public void Clear(Color color)
        {
            g.Clear(color);
        }

        public void UseGraphics(Graphics graphics)
        {
            g = graphics;

            camera.ScreenSize = target.ClientSize;

            var begin = camera.ScreenToWorldSpace(new Vector2(0, 0));
            var end = camera.ScreenToWorldSpace(new Vector2(camera.ScreenSize.Width, camera.ScreenSize.Height));

            ViewPort = new BoundingBox(begin, end);

            updateQuality();
        }

        public void DrawLine(Pen pen, Vector2 point1, Vector2 point2)
        {
            var npen = new Pen(pen.Color, pen.Width * camera.Scale);

            g.DrawLine(npen, (PointF)camera.WorldToScreenSpace(point1), (PointF)camera.WorldToScreenSpace(point2));
        }

        public void DrawGrid(float gridSize, Pen pen)
        {
            float scaledGridSize = gridSize * camera.Scale;

            if (scaledGridSize < 5)
                return;

            var clientSize = target.ClientSize;

            var distToNull = camera.WorldToScreenSpace(Vector2.Zero);
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

        public void FillCircle(Brush brush, Vector2 pos, float radius)
        {
            var drawPos = camera.WorldToScreenSpace(pos);
            float scaledRadius = (radius * camera.Scale);

            g.FillEllipse(brush, drawPos.X - scaledRadius, drawPos.Y - scaledRadius, scaledRadius * 2, scaledRadius * 2);
        }
        public void DrawCircle(Pen pen, Vector2 pos, float radius)
        {
            var npen = new Pen(pen.Color, pen.Width * camera.Scale);

            var drawPos = camera.WorldToScreenSpace(pos);
            float scaledRadius = (radius * camera.Scale);

            g.DrawEllipse(npen, drawPos.X - scaledRadius, drawPos.Y - scaledRadius, scaledRadius * 2, scaledRadius * 2);
        }

        public void DrawRectangle(Pen pen, RectangleF rect)
        {
            var npen = new Pen(pen.Color, pen.Width * camera.Scale);
            var drawPos = camera.WorldToScreenSpace((Vector2)rect.Location);
            float width = rect.Width * camera.Scale;
            float height = rect.Height * camera.Scale;
            g.DrawRectangle(npen, drawPos.X, drawPos.Y, width, height);
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            var drawPos = camera.WorldToScreenSpace((Vector2)rect.Location);
            float width = rect.Width * camera.Scale;
            float height = rect.Height * camera.Scale;
            g.FillRectangle(brush, drawPos.X, drawPos.Y, width, height);
        }

        public void DrawString(string text, Font font, Brush brush, RectangleF rect)
        {
            var nfont = new Font(font.Name, font.Size * camera.Scale);
            var drawPos = camera.WorldToScreenSpace((Vector2)rect.Location);
            float width = rect.Width * camera.Scale;
            float height = rect.Height * camera.Scale;
            var format = new StringFormat(StringFormatFlags.NoWrap) {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            };
            g.DrawString(text, nfont, brush, new RectangleF(drawPos.X, drawPos.Y, width, height), format);
        }

        private void updateQuality()
        {
            if (g == null)
                return;

            if (_highQuality)
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            }
            else
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            }
        }
    }
}

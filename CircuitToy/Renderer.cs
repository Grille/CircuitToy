using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CircuitLib;
using CircuitLib.Math;
using CircuitLib.Interface;
using System.Numerics;

namespace CircuitToy;

internal class Renderer
{
    Camera camera;
    Control target;
    public Circuit Circuit;
    EditorInterface editor;
    Selection selection;
    Graphics g;
    Timer timer;

    BoundingBox view;

    public bool DebugMode = false;
    public bool HighQuality = true;
    public bool ViewGrid = true;

    public Renderer(Control target, Circuit circuit, Camera camera, EditorInterface editor)
    {
        this.target = target;
        this.Circuit = circuit;
        this.camera = camera;
        this.editor = editor;
        this.selection = editor.Selection;

        target.Paint += Target_Paint;
        timer = new Timer();
        timer.Tick += Timer_Tick; ; 
        timer.Interval = 1000 / 70;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        target.Refresh();
    }

    private void Target_Paint(object sender, PaintEventArgs e)
    {
        Render(e.Graphics);
    }

    public void Start()
    {
        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();
    }

    public void Render(Graphics g)
    {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

        camera.ScreenSize = target.ClientSize;

        var begin = camera.ScreenToWorldSpace(new Vector2(0, 0));
        var end = camera.ScreenToWorldSpace(new Vector2(camera.ScreenSize.Width, camera.ScreenSize.Height));

        view.BeginX = begin.X;
        view.BeginY = begin.Y;
        view.EndX = end.X;
        view.EndY = end.Y;

        this.g = g;
        g.Clear(Color.White);

        if (ViewGrid)
        {
            drawGrid();
        }

        if (HighQuality)
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }
        else
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        }

        foreach (var net in Circuit.Networks)
        {
            drawNet(net);
        }

        foreach (var node in Circuit.Nodes)
        {
            drawNode(node);
        }

        if (selection.IsSelectingArea)
        {
            var rect = (RectangleF)selection.SelectetArea;
            fillRectangle(new SolidBrush(Color.FromArgb(50,Color.DarkSeaGreen)), rect);
            drawRectangle(new Pen(Brushes.DarkSeaGreen,0.01f), rect);
        }

        if (true || DebugMode)
        {
            var entity = selection.HoveredEntity;
            if (entity != null)
            {
                var str = new StringBuilder();
                str.AppendLine("name:  "+entity.Name); 
                str.AppendLine("owner: "+entity.Owner?.Name);

                if (entity is IOPin)
                {
                    var iopin = (IOPin)entity;
                    str.AppendLine("net:   "+iopin.ConnectedNetwork?.Name);
                }
                else
                {

                }
                g.DrawString("#\n*\n" + str.ToString(), new Font("consolas", 12.0f), Brushes.Magenta, (PointF)editor.ScreenMousePos);
            }
        }

        g.Flush();
    }

    void drawNode(Node node)
    {
        //if (!node.Bounds.IsColliding(view))
        //    return;

        drawPins(node.InputPins);
        drawPins(node.OutputPins);

        if (DebugMode)
            drawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)node.Bounds);

        float width = node.Size.X;
        float height = node.Size.Y;
        var rect = new RectangleF(node.Position.X - width/2, node.Position.Y - height/2, width, height);
        fillRectangle(Brushes.White, rect);
        if (node.IsHovered)
        {
            drawRectangle(new Pen(Brushes.Lime, 0.2f), rect);
        }
        if (node.IsSelected)
        {
            var srect = rect;
            srect.X += MathF.Round(selection.Offset.X);
            srect.Y += MathF.Round(selection.Offset.Y);
            drawRectangle(new Pen(Brushes.LightSeaGreen, 0.2f), srect);


        }
        drawRectangle(new Pen(Brushes.Black, 0.1f), rect);
        drawString(node.DisplayName, new Font("consolas", 0.6f), Brushes.Black, rect);
    }

    void drawPins(IOPin[] pins)
    {
        foreach (var pin in pins)
        {
            var pos = pin.Position;
            if (DebugMode)
                drawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)pin.Bounds);

            if (pin.Active)
                drawLine(new Pen(Brushes.Blue, 0.1f), pos, pin.Owner.Position);
            else
                drawLine(new Pen(Brushes.Black, 0.1f), pos, pin.Owner.Position);

            if (pin.IsHovered)
            {
                fillCircle(Brushes.Lime, pos, 0.35f);
            }
            if (pin.IsSelected)
            {
                var spos = pos;
                spos.X += MathF.Round(selection.Offset.X);
                spos.Y += MathF.Round(selection.Offset.Y);
                fillCircle(Brushes.LightSeaGreen, spos, 0.35f);

                foreach (var wire in pin.ConnectedWires)
                {
                    PointF point1;
                    PointF point2;

                    if (wire.StartPin.IsSelected)
                    {
                        float X = MathF.Round(selection.Offset.X);
                        float Y = MathF.Round(selection.Offset.Y);
                        point1 = (PointF)camera.WorldToScreenSpace(new Vector2(wire.StartPin.Position.X+X, wire.StartPin.Position.Y + Y));
                    }
                    else
                    {
                        point1 = (PointF)camera.WorldToScreenSpace(wire.StartPin.Position);
                    }

                    if (wire.EndPin.IsSelected)
                    {
                        float X = MathF.Round(selection.Offset.X);
                        float Y = MathF.Round(selection.Offset.Y);
                        point2 = (PointF)camera.WorldToScreenSpace(new Vector2(wire.EndPin.Position.X + X, wire.EndPin.Position.Y + Y));
                    }
                    else
                    {
                        point2 = (PointF)camera.WorldToScreenSpace(wire.EndPin.Position);
                    }


                    g.DrawLine(new Pen(Brushes.LightSeaGreen, 0.2f * camera.Scale), point1, point2);
                }
            }

            if (pin.Active) 
                fillCircle(Brushes.Blue, pos, 0.25f);
            else
                fillCircle(Brushes.Black, pos, 0.25f);

        }
    }

    void drawPins(List<NetPin> pins)
    {
        foreach (var pin in pins)
        {
            var pos = pin.Position;
            if (DebugMode)
                drawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)pin.Bounds);

            if (pin.IsHovered)
            {
                fillCircle(Brushes.Lime, pos, 0.35f);
            }
            if (pin.IsSelected)
            {
                var spos = pos;
                spos.X += MathF.Round(selection.Offset.X);
                spos.Y += MathF.Round(selection.Offset.Y);
                fillCircle(Brushes.LightSeaGreen, spos, 0.35f);

                foreach (var wire in pin.ConnectedWires)
                {
                    PointF point1;
                    PointF point2;

                    if (wire.StartPin.IsSelected)
                    {
                        float X = MathF.Round(selection.Offset.X);
                        float Y = MathF.Round(selection.Offset.Y);
                        point1 = (PointF)camera.WorldToScreenSpace(new Vector2(wire.StartPin.Position.X + X, wire.StartPin.Position.Y + Y));
                    }
                    else
                    {
                        point1 = (PointF)camera.WorldToScreenSpace(wire.StartPin.Position);
                    }

                    if (wire.EndPin.IsSelected)
                    {
                        float X = MathF.Round(selection.Offset.X);
                        float Y = MathF.Round(selection.Offset.Y);
                        point2 = (PointF)camera.WorldToScreenSpace(new Vector2(wire.EndPin.Position.X + X, wire.EndPin.Position.Y + Y));
                    }
                    else
                    {
                        point2 = (PointF)camera.WorldToScreenSpace(wire.EndPin.Position);
                    }


                    g.DrawLine(new Pen(Brushes.LightSeaGreen, 0.2f * camera.Scale), point1, point2);
                }
            }

            if (pin.ConnectedWires.Count != 2)
            {
                if (pin.Active)
                    fillCircle(Brushes.Blue, pos, 0.15f);
                else
                    fillCircle(Brushes.Black, pos, 0.15f);
            }
            else
            {
                if (pin.Active)
                    fillCircle(Brushes.Blue, pos, 0.05f);
                else
                    fillCircle(Brushes.Black, pos, 0.05f);
            }

        }
    }

    void drawNet(Network net)
    {
        foreach (var wire in net.Wires)
        {
            drawWire(wire);
        }
        drawPins(net.GuardPins);

        if (DebugMode)
        {
            foreach (var outPin in net.OutputPins)
            {
                foreach (var inPin in net.InputPins)
                {
                    drawLine(new Pen(Brushes.Magenta, 0.01f), inPin.Position, outPin.Position);
                }
            }

            drawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)net.Bounds);
        }

    }

    void drawWire(Wire wire)
    {
        var point1 = camera.WorldToScreenSpace(wire.StartPin.Position);
        var point2 = camera.WorldToScreenSpace(wire.EndPin.Position);

        if (wire.IsHovered)
            g.DrawLine(new Pen(Brushes.Lime, 0.2f * camera.Scale), (PointF)point1, (PointF)point2);
        if (wire.IsSelected)
            g.DrawLine(new Pen(Brushes.LightSeaGreen, 0.2f * camera.Scale), (PointF)point1, (PointF)point2);

        if (wire.Active)
            g.DrawLine(new Pen(Brushes.Blue, 0.1f * camera.Scale), (PointF)point1, (PointF)point2);
        else
            g.DrawLine(new Pen(Brushes.Black, 0.1f * camera.Scale), (PointF)point1, (PointF)point2);

        if (DebugMode)
            drawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)wire.Bounds);
    }

    void drawGrid()
    {
        drawGrid(1, Pens.WhiteSmoke);
        drawGrid(10, Pens.WhiteSmoke);
        drawGrid(100, Pens.WhiteSmoke);
    }

    void drawLine(Pen pen, Vector2 point1, Vector2 point2)
    {
        var npen = new Pen(pen.Color, pen.Width * camera.Scale);

        g.DrawLine(npen, (PointF)camera.WorldToScreenSpace(point1), (PointF)camera.WorldToScreenSpace(point2));
    }

    void drawGrid(float gridSize, Pen pen)
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

    void fillCircle(Brush brush, Vector2 pos, float radius)
    {
        var drawPos = camera.WorldToScreenSpace(pos);
        float scaledRadius = (radius * camera.Scale);

        g.FillEllipse(brush, drawPos.X - scaledRadius, drawPos.Y - scaledRadius, scaledRadius * 2, scaledRadius * 2);
    }
    void drawCircle(Pen pen,Vector2 pos,float radius)
    {
        var npen = new Pen(pen.Color, pen.Width * camera.Scale);

        var drawPos = camera.WorldToScreenSpace(pos);
        float scaledRadius = (radius * camera.Scale);

        g.DrawEllipse(npen, drawPos.X - scaledRadius, drawPos.Y - scaledRadius, scaledRadius * 2, scaledRadius * 2);
    }

    void drawRectangle(Pen pen,RectangleF rect)
    {
        var npen = new Pen(pen.Color, pen.Width*camera.Scale);
        var drawPos = camera.WorldToScreenSpace((Vector2)rect.Location);
        float width = rect.Width * camera.Scale;
        float height = rect.Height * camera.Scale;
        g.DrawRectangle(npen, drawPos.X, drawPos.Y, width, height);
    }

    void fillRectangle(Brush brush, RectangleF rect)
    {
        var drawPos = camera.WorldToScreenSpace((Vector2)rect.Location);
        float width = rect.Width * camera.Scale;
        float height = rect.Height * camera.Scale;
        g.FillRectangle(brush, drawPos.X, drawPos.Y, width, height);
    }

    void drawString(string text, Font font, Brush brush, RectangleF rect)
    {
        var nfont = new Font(font.Name,font.Size * camera.Scale);
        var drawPos = camera.WorldToScreenSpace((Vector2)rect.Location);
        float width = rect.Width * camera.Scale;
        float height = rect.Height * camera.Scale;
        var format = new StringFormat(StringFormatFlags.NoWrap  ) { 
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };
        g.DrawString(text, nfont, brush, new RectangleF(drawPos.X, drawPos.Y, width, height), format);
    }


}


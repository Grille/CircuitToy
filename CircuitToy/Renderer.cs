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

namespace CircuitToy;

internal class Renderer
{
    Camera camera;
    Control target;
    Circuit circuit;
    EditorInterface editor;
    Selection selection;
    Graphics g;
    Timer timer;

    BoundingBoxF view;

    public bool DebugMode = false;
    public bool HighQuality = true;
    public bool ViewGrid = true;

    public Renderer(Control target, Circuit circuit, Camera camera, EditorInterface editor)
    {
        this.target = target;
        this.circuit = circuit;
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

        var begin = camera.ScreenToWorldSpace(new PointF(0, 0));
        var end = camera.ScreenToWorldSpace(new PointF(camera.ScreenSize.Width, camera.ScreenSize.Height));

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

        foreach (var net in circuit.Networks)
        {
            drawNet(net);
        }

        foreach (var node in circuit.Nodes)
        {
            drawNode(node);
        }

        if (selection.IsSelectingArea)
        {
            var rect = (RectangleF)selection.SelectetArea;
            fillRectangle(new SolidBrush(Color.FromArgb(50,Color.DarkSeaGreen)), rect);
            DrawRectangle(new Pen(Brushes.DarkSeaGreen,0.01f), rect);
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
                g.DrawString("#\n*\n" + str.ToString(), new Font("consolas", 12.0f), Brushes.Magenta, editor.ScreenMousePos);
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
            DrawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)node.Bounds);

        float width = node.Size.Width;
        float height = node.Size.Height;
        var rect = new RectangleF(node.Position.X - width/2, node.Position.Y - height/2, width, height);
        fillRectangle(Brushes.White, rect);
        if (node.IsHovered)
        {
            var hrect = rect;
            hrect.X += selection.Offset.X;
            hrect.Y += selection.Offset.Y;
            DrawRectangle(new Pen(Brushes.Lime, 0.2f), hrect);
        }
        if (node.IsSelected)
        {
            var srect = rect;
            srect.X += MathF.Round(selection.Offset.X);
            srect.Y += MathF.Round(selection.Offset.Y);
            DrawRectangle(new Pen(Brushes.LightSeaGreen, 0.2f), srect);
        }
        DrawRectangle(new Pen(Brushes.Black, 0.1f), rect);
        drawString(node.DisplayName, new Font("consolas", 0.6f), Brushes.Black, rect);
    }

    void drawPins(IOPin[] pins)
    {
        foreach (var pin in pins)
        {
            var pos = pin.Position;
            if (DebugMode)
                DrawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)pin.Bounds);

            if (pin.Active)
                drawLine(new Pen(Brushes.Blue, 0.1f), pos, pin.Owner.Position);
            else
                drawLine(new Pen(Brushes.Black, 0.1f), pos, pin.Owner.Position);

            if (pin.IsHovered)
            {
                var hpos = pos;
                hpos.X += selection.Offset.X;
                hpos.Y += selection.Offset.Y;
                fillCircle(Brushes.Lime, hpos, 0.35f);
            }
            if (pin.IsSelected)
            {
                var spos = pos;
                spos.X += MathF.Round(selection.Offset.X);
                spos.Y += MathF.Round(selection.Offset.Y);
                fillCircle(Brushes.LightSeaGreen, spos, 0.35f);
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
                DrawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)pin.Bounds);

            if (pin.IsHovered)
            {
                var hpos = pos;
                hpos.X += selection.Offset.X;
                hpos.Y += selection.Offset.Y;
                fillCircle(Brushes.Lime, hpos, 0.35f);
            }
            if (pin.IsSelected)
            {
                var spos = pos;
                spos.X += MathF.Round(selection.Offset.X);
                spos.Y += MathF.Round(selection.Offset.Y);
                fillCircle(Brushes.LightSeaGreen, spos, 0.35f);
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

            DrawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)net.Bounds);
        }

    }

    void drawWire(Wire wire)
    {
        var point1 = camera.WorldToScreenSpace(wire.StartPin.Position);
        var point2 = camera.WorldToScreenSpace(wire.EndPin.Position);

        if (wire.IsHovered)
            g.DrawLine(new Pen(Brushes.Lime, 0.2f * camera.Scale), point1, point2);
        if (wire.IsSelected)
            g.DrawLine(new Pen(Brushes.LightSeaGreen, 0.2f * camera.Scale), point1, point2);

        if (wire.Active)
            g.DrawLine(new Pen(Brushes.Blue, 0.1f * camera.Scale), point1, point2);
        else
            g.DrawLine(new Pen(Brushes.Black, 0.1f * camera.Scale), point1, point2);

        if (DebugMode)
            DrawRectangle(new Pen(Brushes.Magenta, 0.01f), (RectangleF)wire.Bounds);
    }

    void drawGrid()
    {
        drawGrid(1, Pens.WhiteSmoke);
        drawGrid(10, Pens.WhiteSmoke);
        drawGrid(100, Pens.WhiteSmoke);
    }

    void drawLine(Pen pen, PointF point1, PointF point2)
    {
        var npen = new Pen(pen.Color, pen.Width * camera.Scale);

        g.DrawLine(npen, camera.WorldToScreenSpace(point1), camera.WorldToScreenSpace(point2));
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

    void fillCircle(Brush brush, PointF pos, float radius)
    {
        var drawPos = camera.WorldToScreenSpace(pos);
        int scaledRadius = (int)(radius * camera.Scale);

        g.FillEllipse(brush, new((int)drawPos.X - scaledRadius, (int)drawPos.Y - scaledRadius, scaledRadius * 2, scaledRadius * 2));
    }
    void drawCircle(Pen pen,PointF pos,float radius)
    {
        var npen = new Pen(pen.Color, pen.Width * camera.Scale);

        var drawPos = camera.WorldToScreenSpace(pos);
        int scaledRadius = (int)(radius * camera.Scale);

        g.DrawEllipse(npen, new((int)drawPos.X - scaledRadius, (int)drawPos.Y - scaledRadius, scaledRadius * 2, scaledRadius * 2));
    }

    void DrawRectangle(Pen pen,RectangleF rect)
    {
        var npen = new Pen(pen.Color, pen.Width*camera.Scale);
        var drawPos = camera.WorldToScreenSpace(rect.Location);
        float width = rect.Width * camera.Scale;
        float height = rect.Height * camera.Scale;
        g.DrawRectangle(npen, drawPos.X, drawPos.Y, width, height);
    }

    void fillRectangle(Brush brush, RectangleF rect)
    {
        var drawPos = camera.WorldToScreenSpace(rect.Location);
        float width = rect.Width * camera.Scale;
        float height = rect.Height * camera.Scale;
        g.FillRectangle(brush, drawPos.X, drawPos.Y, width, height);
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


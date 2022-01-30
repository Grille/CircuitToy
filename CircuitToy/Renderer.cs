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
    Control target;
    public Circuit Circuit;
    EditorInterface editor;
    Selection selection;
    Timer timer;

    RenderCore rc;

    BoundingBox view;

    public bool DebugMode = false;
    public bool HighQuality = true;
    public bool ViewGrid = true;

    const float outlineWidth = 0.1f;
     
    const float textSize = 0.8f;
    const float linewWidth = 0.2f;
    const float linewOutlineWidth = linewWidth + outlineWidth;
     
    const float pin0Radius = 0.1f;
    const float pin0OutlineRadius = pin0Radius + outlineWidth;
    const float pin1Radius = 0.4f;
    const float pin1OutlineRadius = pin1Radius + outlineWidth;

    public Renderer(Control target, Circuit circuit, Camera camera, EditorInterface editor)
    {
        this.target = target;
        this.Circuit = circuit;
        this.editor = editor;
        this.selection = editor.Selection;

        rc = new RenderCore(target, camera);

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

        rc.UseGraphics(g);

        rc.Clear(Color.White);
        
        if (ViewGrid)
        {
            rc.HighQuality = false;
            drawGrid();
        }

        rc.HighQuality = HighQuality;

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
            Console.WriteLine($"boun X{selection.SelectetArea.BeginX} EX{selection.SelectetArea.EndY} W{selection.SelectetArea.getWidth()}");

            Console.WriteLine($"rect X{rect.X} W{rect.Width}");

            rc.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.DarkSeaGreen)), rect);
            rc.DrawRectangle(new Pen(Brushes.DarkSeaGreen, 0.01f), rect);
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
            rc.DrawRectangle(new Pen(Brushes.Magenta, linewWidth), (RectangleF)node.Bounds);

        float width = node.Size.X;
        float height = node.Size.Y;
        var rect = new RectangleF(node.Position.X - width/2, node.Position.Y - height/2, width, height);
        rc.FillRectangle(Brushes.White, rect);
        if (node.IsHovered)
        {
            rc.DrawRectangle(new Pen(Brushes.Lime, linewOutlineWidth), rect);
        }
        if (node.IsSelected)
        {
            var srect = rect;
            srect.X += MathF.Round(selection.Offset.X);
            srect.Y += MathF.Round(selection.Offset.Y);
            rc.DrawRectangle(new Pen(Brushes.LightSeaGreen, linewOutlineWidth), srect);


        }
        rc.DrawRectangle(new Pen(Brushes.Black, linewWidth), rect);
        rc.DrawString(node.DisplayName, new Font("consolas", textSize), Brushes.Black, rect);
    }

    void drawPins(IOPin[] pins)
    {
        foreach (var pin in pins)
        {
            var pos = pin.Position;
            if (DebugMode)
                rc.DrawRectangle(new Pen(Brushes.Magenta, linewWidth), (RectangleF)pin.Bounds);

            if (pin.Active)
                rc.DrawLine(new Pen(Brushes.Blue, linewWidth), pos, pin.LineEndPosition);
            else
                rc.DrawLine(new Pen(Brushes.Black, linewWidth), pos, pin.LineEndPosition);

            if (pin.IsHovered)
            {
                rc.FillCircle(Brushes.Lime, pos, pin1OutlineRadius);
            }
            if (pin.IsSelected)
            {
                var spos = pos;
                spos.X += MathF.Round(selection.Offset.X);
                spos.Y += MathF.Round(selection.Offset.Y);
                rc.FillCircle(Brushes.LightSeaGreen, spos, pin1OutlineRadius);

                /*
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
                */
            }

            if (pin.Active) 
                rc.FillCircle(Brushes.Blue, pos, pin1Radius);
            else
                rc.FillCircle(Brushes.Black, pos, pin1Radius);

        }
    }

    void drawPins(List<NetPin> pins)
    {
        foreach (var pin in pins)
        {
            var pos = pin.Position;
            if (DebugMode)
                rc.DrawRectangle(new Pen(Brushes.Magenta, linewWidth), (RectangleF)pin.Bounds);

            if (pin.IsHovered)
            {
                rc.FillCircle(Brushes.Lime, pos, pin1OutlineRadius);
            }
            if (pin.IsSelected)
            {
                var spos = pos;
                spos.X += MathF.Round(selection.Offset.X);
                spos.Y += MathF.Round(selection.Offset.Y);
                rc.FillCircle(Brushes.LightSeaGreen, spos, pin1OutlineRadius);

                /*
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
                */

            }

            if (pin.ConnectedWires.Count != 2)
            {
                if (pin.Active)
                    rc.FillCircle(Brushes.Blue, pos, pin0OutlineRadius);
                else
                    rc.FillCircle(Brushes.Black, pos, pin0OutlineRadius);
            }
            else
            {
                if (pin.Active)
                    rc.FillCircle(Brushes.Blue, pos, pin0Radius);
                else
                    rc.FillCircle(Brushes.Black, pos, pin0Radius);
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
                    rc.DrawLine(new Pen(Brushes.Magenta, linewWidth), inPin.Position, outPin.Position);
                }
            }

            rc.DrawRectangle(new Pen(Brushes.Magenta, linewWidth), (RectangleF)net.Bounds);
        }

    }

    void drawWire(Wire wire)
    {
        if (wire.Active)
            rc.DrawLine(new Pen(Brushes.Blue, linewWidth), wire.StartPin.Position, wire.EndPin.Position);
        else
            rc.DrawLine(new Pen(Brushes.Black, linewWidth), wire.StartPin.Position, wire.EndPin.Position);

        if (DebugMode)
            rc.DrawRectangle(new Pen(Brushes.Magenta, linewWidth), (RectangleF)wire.Bounds);
        
    }

    void drawGrid()
    {
        rc.DrawGrid(1, Pens.WhiteSmoke);
        rc.DrawGrid(10, Pens.WhiteSmoke);
        rc.DrawGrid(100, Pens.WhiteSmoke);
    }




}


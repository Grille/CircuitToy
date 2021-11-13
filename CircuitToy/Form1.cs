using System;
using System.Windows.Forms;
using CircuitLib;
using CircuitLib.Primitives;

namespace CircuitToy;
public partial class Form1 : Form
{
    Circuit circuit;
    Camera camera;
    Renderer renderer;
    Timer timer;

    public Form1()
    {
        InitializeComponent();
        DoubleBuffered = true;
        circuit = new Circuit();
        circuit.Nodes.Add(new AndGate() { 
            Position = new(10, 10) 
        });
        circuit.Nodes.Add(new XorGate() {
            Position = new(5, 9)
        });
        Control renderTarget = pictureBox1;
        camera = new Camera();
        renderer = new Renderer(renderTarget, camera);
        renderTarget.MouseWheel += Panel1_MouseWheel;
        renderTarget.MouseMove += Panel1_MouseMove;

        renderTarget.Paint += RenderTarget_Paint;
        timer = new Timer();
        timer.Tick += Timer_Tick; ;
        timer.Interval = 1000 / 70;
        timer.Start();
    }

    private void RenderTarget_Paint(object? sender, PaintEventArgs e)
    {
        renderer.Render(e.Graphics, circuit);
    }

    private void Timer_Tick(object? sender, System.EventArgs e)
    {
        pictureBox1.Refresh();
    }

    private void Panel1_MouseMove(object? sender, MouseEventArgs e)
    {
        camera.MouseMoveEvent(e, e.Button.HasFlag(MouseButtons.Middle));
    }

    private void Panel1_MouseWheel(object? sender, MouseEventArgs e)
    {
        camera.MouseScrollEvent(e, 1.5f);
    }
}


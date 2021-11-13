using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CircuitLib;
using CircuitLib.Primitives;

namespace CircuitToy;

internal class Simulation
{
    public Circuit Circuit;
    public Camera Camera;
    public Renderer Renderer;
    public Control Target;

    public Simulation(Control target)
    {
        Target = target;

        Circuit = new Circuit();
        {
            var input0 = Circuit.CreateNode<Input>(0, 0);
            var input1 = Circuit.CreateNode<Input>(0, 4);
            var output = Circuit.CreateNode<Output>(10, 2);
            var orgate = Circuit.CreateNode<OrGate>(5, 2);

            input0.ConnectTo(orgate, 0, 0);
            input1.ConnectTo(orgate, 0, 1);
            orgate.ConnectTo(output, 0, 0);

            input0.Active = true;
            input0.Update();
        }

        Camera = new Camera();
        Renderer = new Renderer(Target, Circuit, Camera);
        Renderer.Start();

        Target.MouseMove += Target_MouseMove;
        Target.MouseWheel += Target_MouseWheel;
        Target.MouseDown += Target_MouseDown;

    }

    void add()
    {
        Circuit.CreateNode<OrGate>(5, 2);
    }


    private void Target_MouseWheel(object? sender, MouseEventArgs e)
    {
        Camera.MouseScrollEvent(e, 1.5f);
    }

    private void Target_MouseMove(object? sender, MouseEventArgs e)
    {
        Camera.MouseMoveEvent(e, e.Button.HasFlag(MouseButtons.Middle));
        var pos = Camera.ScreenToWorldSpace(e.Location);
        Circuit.HoverAt(pos);
    }

    private void Target_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            var pos = Camera.ScreenToWorldSpace(e.Location);
            //Circuit.HoverAt(pos);
            //Circuit.CreateNode<OrGate>(MathF.Round(pos.X), MathF.Round(pos.Y));
        }
    }
}

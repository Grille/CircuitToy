using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using CircuitLib;
using CircuitLib.Primitives;
using CircuitLib.Interface;

namespace CircuitToy;

internal class Simulation
{
    public Circuit Circuit;
    public Camera Camera;
    public Renderer Renderer;
    public Control Target;
    public Selection Selection;
    public EditorInterface Interaction;

    public ContextMenuStrip ContextMenu;
    public PointF MouseUpPos = Point.Empty;

    public Simulation(Control target)
    {
        Target = target;

        Circuit = new Circuit();
        {
            var input0 = Circuit.CreateNode<Input>(0, 0);
            var input1 = Circuit.CreateNode<Input>(0, 4);
            var output = Circuit.CreateNode<Output>(10, 2);
            var orgate = Circuit.CreateNode<AndGate>(5, 2);

            input0.ConnectTo(orgate, 0, 0);
            input1.ConnectTo(orgate, 0, 1);
            orgate.ConnectTo(output, 0, 0);

            //input0.OutputPins[0].ConnectedNetwork.ConnectFromTo(input0.OutputPins[0], new PointF(-10, -10));
            input0.Active = true;
            input0.Update();

            input1.Active = false;
            input1.Update();
        }

        Camera = new Camera();
        Selection = new Selection(Circuit);
        Interaction = new EditorInterface(Circuit, Camera, Selection);

        Renderer = new Renderer(Target, Circuit, Camera, Interaction);
        Renderer.DebugMode = false;
        Renderer.Start();

        Target.MouseMove += Target_MouseMove;
        Target.MouseWheel += Target_MouseWheel;
        Target.MouseDown += Target_MouseDown;
        Target.MouseUp += Target_MouseUp;

        Target.KeyDown += Target_KeyDown;



    }

    private void Target_KeyDown(object sender, KeyEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Target_MouseWheel(object sender, MouseEventArgs e)
    {
        Camera.MouseScrollEvent(e.Location,e.Delta, 1.5f);
    }

    private void Target_MouseMove(object sender, MouseEventArgs e)
    {
        Camera.MouseMoveEvent(e.Location, e.Button.HasFlag(MouseButtons.Middle));
        Interaction.MouseMove(e.Location, e.Button.HasFlag(MouseButtons.Left));
    }

    private void Target_MouseDown(object sender, MouseEventArgs e)
    {
        Interaction.MouseDown(e.Location, e.Button.HasFlag(MouseButtons.Left));
    }

    private void Target_MouseUp(object sender, MouseEventArgs e)
    {
        Interaction.MouseUp(e.Location, e.Button.HasFlag(MouseButtons.Left));
    }
}

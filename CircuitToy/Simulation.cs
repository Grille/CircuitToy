using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Numerics;

using CircuitLib;
using CircuitLib.Primitives;
using CircuitLib.Interface;
using CircuitLib.Rendering;
using CircuitLib.Serialization;

namespace CircuitToy;

internal class Simulation
{
    public Form Form;
    public Circuit Circuit;
    public Camera Camera;
    public RendererGdiBackend RendererBackend;
    public Renderer Renderer;
    public Control Target;
    public EditorInterface Interaction;
    public Theme Theme;

    string path = "newcircuit.lcp";

    public Simulation(Form form, Control target)
    {
        Form = form;
        Target = target;

        var andcirc = new Circuit();
        {
            var input0 = andcirc.CreateNode<Input>(0, 0);
            var input1 = andcirc.CreateNode<Input>(0, 4);
            var output = andcirc.CreateNode<Output>(10, 2);
            var orgate = andcirc.CreateNode<AndGate>(5, 2);
            input0.ConnectTo(orgate, 0, 0);
            input1.ConnectTo(orgate, 0, 1);
            orgate.ConnectTo(output, 0, 0);
            andcirc.UpdateIO();

            andcirc.Position = new Vector2(5, 2);
            andcirc.Size = new Vector2(2, 2);
            andcirc.DisplayName = "AN";
        }

        Circuit = new Circuit();
        {
            var input0 = Circuit.CreateNode<Input>(0, 0);
            var input1 = Circuit.CreateNode<Input>(0, 4);
            var output = Circuit.CreateNode<Output>(10, 2);
            var orgate = andcirc;
            Circuit.AddNode(orgate);

            input0.ConnectTo(orgate, 0, 0);
            input1.ConnectTo(orgate, 0, 1);
            orgate.ConnectTo(output, 0, 0);

            Circuit.UpdateIO();

            //Circuit.InputPins[0].Active = true;
            //Circuit.InputPins[1].Active = true;
        }

        
        {
            var c = new Circuit();

            var input0set = c.CreateNode<Input>(0, 0);
            var input1reset = c.CreateNode<Input>(0, 4);
            var output = c.CreateNode<Output>(10, 2);

            var norgate0 = c.CreateNode<NOrGate>(5, -5);
            var norgate1 = c.CreateNode<NOrGate>(5, 5);


            input0set.ConnectTo(norgate0, 0, 0);

            input1reset.ConnectTo(norgate1, 0, 1);

            norgate0.ConnectTo(norgate1, 0, 0);

            norgate1.ConnectTo(norgate0, 0, 1);
            norgate1.ConnectTo(output, 0, 0);

            Circuit = c;
        }


        Theme = new Theme() {
            SceneBackColor = Color.White,
            SceneGridColor = Color.WhiteSmoke,

            NodeBackColor = Color.LightGray,
            NodeTextColor = Color.Black,

            StateLowColor = Color.Black,
            StateHighColor = Color.Blue,
            StateOffColor = Color.FromArgb(150,150,150),
            StateErrorColor = Color.Red,

            SelectionColor = Color.Green,
            HoverColor = Color.LimeGreen,
            DebugColor = Color.Lime,
        };



        Camera = new Camera();
        Interaction = new EditorInterface(Circuit, Camera);


        RendererBackend = new RendererGdiBackend();
        Renderer = new Renderer(RendererBackend, Camera, Theme, Interaction, Circuit);

        //Renderer.DebugMode = false;
        //Renderer.Start();

        Target.MouseMove += Target_MouseMove;
        Target.MouseWheel += Target_MouseWheel;
        Target.MouseDown += Target_MouseDown;
        Target.MouseUp += Target_MouseUp;

        Target.KeyDown += Target_KeyDown;
    }

    public void Refresh(Graphics g)
    {
        RendererBackend.UseGraphics(g);


        Camera.ScreenSize = Target.ClientSize;

        Renderer.Render();
    }
    public void New()
    {
        useCircuit(new Circuit());
    }
    public void Save()
    {
        SaveFile.Save(path,Circuit);
    }
    public void Save(string path)
    {
        this.path = path;
        SaveFile.Save(path, Circuit);
    }

    public void Load(string path)
    {
        this.path = path;
        var result = SaveFile.Load(path);
        useCircuit(result.Circuit);
        if (result.State != SaveFileState.OK)
            MessageBox.Show(Form, result.State.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void useCircuit(Circuit circuit)
    {
        Circuit?.Destroy();
        Circuit = circuit;
        Interaction.Circuit = circuit;
        Interaction.Selection.Circuit = circuit;
        Renderer.Circuit = circuit;

        Interaction.Clear();
        
    }

    private void Target_KeyDown(object sender, KeyEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Target_MouseWheel(object sender, MouseEventArgs e)
    {
        Camera.MouseScrollEvent((Vector2)(PointF)e.Location,e.Delta, 1.5f);
    }

    private void Target_MouseMove(object sender, MouseEventArgs e)
    {
        Camera.MouseMoveEvent((Vector2)(PointF)e.Location, e.Button.HasFlag(MouseButtons.Middle));
        Interaction.MouseMove((Vector2)(PointF)e.Location, e.Button.HasFlag(MouseButtons.Left));
    }

    private void Target_MouseDown(object sender, MouseEventArgs e)
    {
        Interaction.MouseDown((Vector2)(PointF)e.Location, e.Button.HasFlag(MouseButtons.Left));
    }

    private void Target_MouseUp(object sender, MouseEventArgs e)
    {
        Interaction.MouseUp((Vector2)(PointF)e.Location, e.Button.HasFlag(MouseButtons.Left));
    }
}

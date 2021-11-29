using System;
using System.Windows.Forms;
using CircuitLib;
using CircuitLib.Primitives;
using CircuitLib.Interface;

namespace CircuitToy;
public partial class MainWindow : Form
{
    Simulation sim;

    public MainWindow()
    {
        InitializeComponent();
        DoubleBuffered = true;

        sim = new Simulation(canvas);
        sim.ContextMenu = contextMenuStrip1;
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode.HasFlag(Keys.ShiftKey))
            sim.Interaction.IsShiftKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.Alt))
            sim.Interaction.IsAltKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.Delete))
        {
            foreach (var obj in sim.Interaction.Selection.SelectedEntities)
            {
                Console.WriteLine(obj.ToString());
                obj.Destroy();
            }
        }

    }

    private void MainWindow_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode.HasFlag(Keys.ShiftKey))
            sim.Interaction.IsShiftKeyDown = false;

        if (e.KeyCode.HasFlag(Keys.Alt))
            sim.Interaction.IsAltKeyDown = false;
    }

    private void iNToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<Input>(sim.MouseUpPos.X, sim.MouseUpPos.Y);
    }

    private void oUTToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<Output>(sim.MouseUpPos.X, sim.MouseUpPos.Y);
    }

    private void oRToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<OrGate>(sim.MouseUpPos.X, sim.MouseUpPos.Y);
    }

    private void aNDToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<AndGate>(sim.MouseUpPos.X, sim.MouseUpPos.Y);
    }

    private void nOTToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<NotGate>(sim.MouseUpPos.X, sim.MouseUpPos.Y);
    }

    private void xORToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<XorGate>(sim.MouseUpPos.X, sim.MouseUpPos.Y);
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog();
        dialog.AddExtension = true;
        dialog.DefaultExt = "lcp";
        dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var result = dialog.ShowDialog(this);
        if (result == DialogResult.OK)
        {
            sim.Save(dialog.FileName);
        }
    }

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.New();
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {

        using var dialog = new OpenFileDialog();
        dialog.AddExtension = true;
        dialog.DefaultExt = "lcp";
        dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var result = dialog.ShowDialog(this);
        if (result == DialogResult.OK)
        {
            sim.Load(dialog.FileName);
        }
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Save();
    }

    private void toolStripBtnMove_Click(object sender, EventArgs e)
    {
        toolStripBtnMove.Checked = true;
        toolStripBtnWire.Checked = false;
        toolStripBtnOnoff.Checked = false;
        sim.Interaction.Mode = ToolMode.SelectAndMove;
    }

    private void toolStripBtnWire_Click(object sender, EventArgs e)
    {
        toolStripBtnMove.Checked = false;
        toolStripBtnWire.Checked = true;
        toolStripBtnOnoff.Checked = false;
        sim.Interaction.Mode = ToolMode.AddWire;
    }

    private void toolStripBtnOnoff_Click(object sender, EventArgs e)
    {
        toolStripBtnMove.Checked = false;
        toolStripBtnWire.Checked = false;
        toolStripBtnOnoff.Checked = true;
        sim.Interaction.Mode = ToolMode.OnOff;
    }
}


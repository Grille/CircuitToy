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
        canvas.ContextMenuStrip = contextMenuStrip1;

        var node = treeView1.Nodes.Add("primitives");
        node.Expand();
        node.Nodes.Add("IN");
        node.Nodes.Add("OUT");
        node.Nodes.Add("BUF");
        node.Nodes.Add("NOT");
        node.Nodes.Add("AND");
        node.Nodes.Add("OR");
        node.Nodes.Add("XOR");
        node.Nodes.Add("NAND");
        node.Nodes.Add("NOR");
        node.Nodes.Add("XNOR");

        treeView1.Nodes.Add("std");
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode.HasFlag(Keys.ShiftKey))
            sim.Interaction.IsShiftKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.ControlKey))
            sim.Interaction.IsCtrlKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.Alt))
            sim.Interaction.IsAltKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.Delete))
        {
            foreach (var obj in sim.Interaction.Selection.SelectedEntities)
            {
                Console.WriteLine(obj.ToString());
                obj.Destroy();
            }
            sim.Interaction.Selection.ClearSelection();
        }

    }

    private void MainWindow_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode.HasFlag(Keys.ShiftKey))
            sim.Interaction.IsShiftKeyDown = false;

        if (e.KeyCode.HasFlag(Keys.ControlKey))
            sim.Interaction.IsCtrlKeyDown = false;

        if (e.KeyCode.HasFlag(Keys.Alt))
            sim.Interaction.IsAltKeyDown = false;
    }

    private void iNToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<Input>(sim.Interaction.WorldMousePos);
    }

    private void oUTToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<Output>(sim.Interaction.WorldMousePos);
    }

    private void oRToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<OrGate>(sim.Interaction.WorldMousePos);
    }

    private void aNDToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<AndGate>(sim.Interaction.WorldMousePos);
    }

    private void nOTToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<NotGate>(sim.Interaction.WorldMousePos);
    }

    private void xORToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<XorGate>(sim.Interaction.WorldMousePos);
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

    private void nANDToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<NAndGate>(sim.Interaction.WorldMousePos);
    }

    private void nORToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<NOrGate>(sim.Interaction.WorldMousePos);
    }

    private void xNORToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<XNorGate>(sim.Interaction.WorldMousePos);
    }

    private void MainWindow_Load(object sender, EventArgs e)
    {

    }

    private void RefreshTimer_Tick(object sender, EventArgs e)
    {
        canvas.Refresh();
    }

    private void canvas_Paint(object sender, PaintEventArgs e)
    {
        sim.Refresh(e.Graphics);
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
        sim.Circuit.Reset();
    }

    private void bUFToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<BufferGate>(sim.Interaction.WorldMousePos);
    }

    private void triStateToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<TriState>(sim.Interaction.WorldMousePos);
    }

    private void pullUpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<PullUp>(sim.Interaction.WorldMousePos);
    }

    private void pullDownToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Circuit.CreateNode<PullDown>(sim.Interaction.WorldMousePos);
    }
}


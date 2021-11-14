using System;
using System.Windows.Forms;
using CircuitLib;
using CircuitLib.Primitives;

namespace CircuitToy;
public partial class MainWindow : Form
{
    Simulation sim;

    public MainWindow()
    {
        InitializeComponent();
        DoubleBuffered = true;

        sim = new Simulation(renderBox);
        sim.ContextMenu = contextMenuStrip1;
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode.HasFlag(Keys.ShiftKey))
            sim.Interaction.IsShiftKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.Alt))
            sim.Interaction.IsAltKeyDown = true;
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
}


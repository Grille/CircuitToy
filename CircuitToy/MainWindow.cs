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
        KeyPreview = true;
        DoubleBuffered = true;

        sim = new Simulation(this, canvas);
        canvas.ContextMenuStrip = contextMenuStrip1;

        {
            var node = treeView1.Nodes.Add("IO");
            node.Expand();
            var nodes = node.Nodes;
            nodes.Add("IN");
            nodes.Add("OUT");
        }
        {
            var node = treeView1.Nodes.Add("Logic Gates");
            node.Expand();
            var nodes = node.Nodes;
            nodes.Add("BUF");
            nodes.Add("NOT");
            nodes.Add("AND");
            nodes.Add("OR");
            nodes.Add("XOR");
            nodes.Add("NAND");
            nodes.Add("NOR");
            nodes.Add("XNOR");
        }
        {
            var node = treeView1.Nodes.Add("Try-State");
            node.Expand();
            var nodes = node.Nodes;
            nodes.Add("Try-State");
            nodes.Add("Pull-Up");
            nodes.Add("Pull-Down");
        }
        {
            var node = treeView1.Nodes.Add("Special");
            node.Expand();
            var nodes = node.Nodes;
            nodes.Add("Delay");
            nodes.Add("Logger");
        }

        treeView1.Nodes.Add("std");
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (canvas.Focused)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                Console.WriteLine("copy");
                var result = sim.Editor.CopySelection();
                var data = new DataObject();
                data.SetData(result);
                Clipboard.SetDataObject(data, true);

                return true;
            }

            if (keyData == (Keys.Control | Keys.X))
            {
                var result = sim.Editor.CutSelection();
                var data = new DataObject();
                data.SetData(result);
                Clipboard.SetDataObject(data, true);

                return true;
            }

            if (keyData == (Keys.Control | Keys.V))
            {
                var data = Clipboard.GetDataObject() as DataObject;
                if (data == null || !data.GetDataPresent(typeof(byte[])))
                    return false;
                var buffer = data.GetData(typeof(byte[])) as byte[];
                sim.Editor.Paste(buffer);

                return true;
            }

            if (keyData == (Keys.Control | Keys.Z))
            {
                sim.Editor.Undo();

                return true;
            }

            if (keyData == (Keys.Control | Keys.Y))
            {
                sim.Editor.Redo();

                return true;
            }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode.HasFlag(Keys.ShiftKey))
            sim.Editor.IsShiftKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.ControlKey))
            sim.Editor.IsCtrlKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.Alt))
            sim.Editor.IsAltKeyDown = true;

        if (e.KeyCode.HasFlag(Keys.Delete))
        {
            sim.Editor.DestroySelection();
        }
    }

    private void MainWindow_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode.HasFlag(Keys.ShiftKey))
            sim.Editor.IsShiftKeyDown = false;

        if (e.KeyCode.HasFlag(Keys.ControlKey))
            sim.Editor.IsCtrlKeyDown = false;

        if (e.KeyCode.HasFlag(Keys.Alt))
            sim.Editor.IsAltKeyDown = false;
    }

    private void iNToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<Input>();
    }

    private void oUTToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<Output>();
    }

    private void oRToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<OrGate>();
    }

    private void aNDToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<AndGate>();
    }

    private void nOTToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<NotGate>();
    }

    private void xORToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<XorGate>();
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
        sim.Editor.Mode = ToolMode.SelectAndMove;
    }

    private void toolStripBtnWire_Click(object sender, EventArgs e)
    {
        toolStripBtnMove.Checked = false;
        toolStripBtnWire.Checked = true;
        toolStripBtnOnoff.Checked = false;
        sim.Editor.Mode = ToolMode.AddWire;
    }

    private void toolStripBtnOnoff_Click(object sender, EventArgs e)
    {
        toolStripBtnMove.Checked = false;
        toolStripBtnWire.Checked = false;
        toolStripBtnOnoff.Checked = true;
        sim.Editor.Mode = ToolMode.OnOff;
    }

    private void nANDToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<NAndGate>();
    }

    private void nORToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<NOrGate>();
    }

    private void xNORToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<XNorGate>();
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
        sim.Editor.CreateNode<BufferGate>();
    }

    private void triStateToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<TriState>();
    }

    private void pullUpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<PullUp>();
    }

    private void pullDownToolStripMenuItem_Click(object sender, EventArgs e)
    {
        sim.Editor.CreateNode<PullDown>();
    }

    private void canvas_MouseEnter(object sender, EventArgs e)
    {
        canvas.Focus();
    }

    private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
    {

    }

    private void darkStatusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {

    }
}


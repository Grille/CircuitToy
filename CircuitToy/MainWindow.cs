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
    }

}


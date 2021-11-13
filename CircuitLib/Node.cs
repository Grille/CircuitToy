using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public abstract class Node
{
    public InputPin[] InputPins;
    public OutputPin[] OutputPins;

    public string Name = "";
    public string Description = "";
    public bool Active = false;
    public Point Position = Point.Empty;

    public abstract void Update();
}


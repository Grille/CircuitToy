using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public class Network : World
{
    public Circuit Owner;

    public List<InputPin> InputPins = new List<InputPin>();
    public List<OutputPin> OutputPins = new List<OutputPin>();

    public bool Active = false;

    //public static Network Ground = new Network();
    public void Connect(OutputPin exitPoint)
    {
        if (!OutputPins.Contains(exitPoint))
        {
            OutputPins.Add(exitPoint);
            exitPoint.Network = this;
        }
    }

    public void Connect(InputPin entryPoint)
    {
        if (!InputPins.Contains(entryPoint))
            InputPins.Add(entryPoint);
            entryPoint.Network = this;
    }

    public override float DistanceTo(PointF pos)
    {
        return 0;
    }

    public void Update()
    {
        Active = false;

        for (int i = 0; i < OutputPins.Count; i++)
        {
            if (OutputPins[i].Active)
            {
                Active = true;
            }
        }

        for (int i = 0; i < InputPins.Count; i++)
        {
            InputPins[i].Active = Active;
        }
    }
}


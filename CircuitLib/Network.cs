using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public class Network
{
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


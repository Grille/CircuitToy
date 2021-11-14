using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public class Network : Entity
{
    public Circuit Owner;

    public List<InputPin> InputPins = new List<InputPin>();
    public List<OutputPin> OutputPins = new List<OutputPin>();

    public bool Active = false;

    public override PointF Position {
        get { return Owner.Position; }
        set { Owner.Position = value; }
    }
    //public static Network Ground = new Network();
    public void Add(OutputPin pin)
    {
        if (!OutputPins.Contains(pin))
        {
            OutputPins.Add(pin);
            pin.Network = this;
        }
    }

    public void Add(InputPin pin)
    {
        if (!InputPins.Contains(pin))
            InputPins.Add(pin);
            pin.Network = this;
    }

    public void Remove(OutputPin pin)
    {
        OutputPins.Remove(pin);
    }

    public void Remove(InputPin pin)
    {
        InputPins.Remove(pin);
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

    public override void CalcBoundings()
    {
        throw new NotImplementedException();
    }
}


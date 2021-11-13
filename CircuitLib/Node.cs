using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public abstract class Node : World
{
    public Circuit Owner;

    public InputPin[] InputPins;
    public OutputPin[] OutputPins;

    public int ID = -1;
    public string Name = "";
    public string Description = "";
    public bool Active = false;
    public SizeF Size = new SizeF(1, 1);

    private PointF _pos;
    public PointF Position {
        get { return _pos; }
        set { 
            _pos = value;
            foreach (var pin in InputPins)
            {
                pin.UpdatePosition();
            }
            foreach (var pin in OutputPins)
            {
                pin.UpdatePosition();
            }
        }
    }

    public void SetSize(float size)
    {
        Size = new SizeF(size, size);
    }


    public override float DistanceTo(PointF pos)
    {
        return 0;
    }

    public abstract void Update();

    public void ConnectTo(Node target, int outId, int inId)
    {
        Network net;
        if (target.InputPins[inId].Network == null)
        {
            net = Owner.CreateNet();
            net.Connect(target.InputPins[inId]);
        }
        else
        {
            net = target.InputPins[inId].Network;
        }

        net.Connect(OutputPins[outId]);
    }
}


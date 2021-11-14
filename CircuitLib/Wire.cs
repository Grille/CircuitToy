using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;

namespace CircuitLib
{
    public class Wire : Entity
    {
        public Pin StartPin;
        public Pin EndPin;

        public new Network Owner {
            get {
                return (Network)base.Owner;
            }
            set {
                base.Owner = value;
            }
        }

        public Wire(Network owner, Pin start, Pin end)
        {
            Owner = owner;
            StartPin = start;
            EndPin = end;
            StartPin.ConnectedWires.Add(this);
            EndPin.ConnectedWires.Add(this);
            CalcBoundings();
        }


        PointF _pos;
        public override PointF Position {
            get => new PointF((StartPin.Position.X+EndPin.Position.X)/2f, (StartPin.Position.Y + EndPin.Position.Y) / 2f);
            set =>  _pos = value;
        }

        private bool _active = false;
        public override bool Active {
            get => _active;
            set => _active = value;
        }

        public override void CalcBoundings()
        {
            const float margin = 0.2f;
            Bounds = new Math.BoundingBoxF(
                MathF.Min(StartPin.Position.X, EndPin.Position.X) - margin,
                MathF.Max(StartPin.Position.X, EndPin.Position.X) + margin,
                MathF.Min(StartPin.Position.Y, EndPin.Position.Y) - margin,
                MathF.Max(StartPin.Position.Y, EndPin.Position.Y) + margin
            );
        }

        public override void Destroy()
        {
            StartPin.ConnectedWires.Remove(this);
            EndPin.ConnectedWires.Remove(this);
            if (EndPin.Owner == this)
                EndPin.Destroy();
        }

        public override Entity GetAt(PointF pos0)
        {
            if (Bounds.IsInside(pos0))
            {
                var pos1 = StartPin.Position;
                var pos2 = EndPin.Position;
                float a = (pos2.X - pos1.X) * (pos1.Y - pos0.Y) - (pos1.X - pos0.X) * (pos2.Y - pos1.Y);
                float b = MathF.Sqrt(MathF.Pow(pos2.X - pos1.X, 2) + MathF.Pow(pos2.Y - pos1.Y, 2));
                float distToLine = MathF.Abs(a / b);

                if (distToLine < 0.1f)
                {
                    return this;
                }
            }
            return null;
        }

        public override void GetFromArea(List<Entity> entities, BoundingBoxF region)
        {
            return;
        }
    }
}

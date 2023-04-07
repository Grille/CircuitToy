using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CircuitLib;

public partial class Network : AsyncUpdatableEntity
{
    public class PinList : List<Pin>
    {
        private readonly Network owner;
        public readonly PinSubList<InputPin> InputPins;
        public readonly PinSubList<OutputPin> OutputPins;
        public readonly PinSubList<WirePin> NetPins;

        public PinList(Network owner)
        {
            this.owner = owner;

            InputPins = new PinSubList<InputPin>(owner, this);
            OutputPins = new PinSubList<OutputPin>(owner, this);
            NetPins = new PinSubList<WirePin>(owner, this);

        }

        public new void Add(Pin pin)
        {
            if (pin.ConnectedNetwork != null)
                throw new InvalidOperationException($"{pin.GetType().FullName} already in other Network");
            if (Contains(pin))
                throw new InvalidOperationException($"{pin.GetType().FullName} already in this Network");

            if (pin is InputPin)
            {
                var inPin = (InputPin)pin;
                inPin.ConnectedNetwork = owner;
                InputPins.Add(inPin);
            }
            else if (pin is OutputPin)
            {
                var outPin = (OutputPin)pin;
                outPin.ConnectedNetwork = owner;
                OutputPins.Add(outPin);
            }
            else if (pin is WirePin)
            {
                var netPin = (WirePin)pin;
                netPin.ConnectedNetwork = netPin.Owner = owner;
                NetPins.Add(netPin);
            }
            else
            {
                throw new ArgumentException($"{pin.GetType().FullName} is not of type Pin");
            }
            base.Add(pin);

            owner.Cleanup();
        }

        public new void Remove(Pin pin)
        {
            if (!Contains(pin))
                throw new InvalidOperationException($"{pin.GetType().FullName} not in this Network");

            if (pin is InputPin)
            {
                var inPin = (InputPin)pin;
                inPin.ConnectedNetwork = null;
                InputPins.Remove(inPin);
                inPin.State = owner.Owner.DefualtState;
                inPin.Owner.Update();
            }
            else if (pin is OutputPin)
            {
                var outPin = (OutputPin)pin;
                outPin.ConnectedNetwork = null;
                OutputPins.Remove(outPin);
            }
            else if (pin is WirePin)
            {
                var netPin = (WirePin)pin;
                netPin.ConnectedNetwork = netPin.Owner = null;
                NetPins.Remove(netPin);
            }
            else
            {
                throw new ArgumentException($"{pin.GetType().FullName} is not of type Pin");
            }
            base.Remove(pin);

            owner.Cleanup();
        }

        public WirePin Create(float x, float y) => Create(new Vector2(x, y));
        public WirePin Create() => Create(Vector2.Zero);
        public WirePin Create(Vector2 pos)
        {
            var pin = new WirePin(owner, pos.X, pos.Y);
            pin.CalcBoundings();
            Add(pin);
            return pin;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public partial class Network : AsyncUpdatableEntity
{
    public class PinSubList<T> : List<T> where T : Pin
    {
        private PinList pins;
        private Network owner;

        public PinSubList(Network owner, PinList pins)
        {
            this.owner = owner;
            this.pins = pins;
        }
    }
}

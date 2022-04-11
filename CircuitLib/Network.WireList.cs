using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public partial class Network : AsyncUpdatableEntity
{
    public class WireList : List<Wire>
    {
        private Network owner;

        public WireList(Network owner)
        {
            this.owner = owner;
        }

        public new void Add(Wire wire)
        {
            base.Add(wire);
            wire.Owner = owner;

            owner.Cleanup();
        }

        public new void Remove(Wire wire)
        {
            base.Remove(wire);
            wire.Owner = null;

            owner.Cleanup();
        }
    }
}

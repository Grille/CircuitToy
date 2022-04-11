using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CircuitLib;

public partial class Circuit : Node
{
    public class NetList : List<Network>
    {
        private Circuit owner;
        public NetList(Circuit owner)
        {
            this.owner = owner;
        }

        public new void Add(Network node)
        {
            node.Owner = owner;
            base.Add(node);
        }

        public Network Create()
        {
            var net = new Network();
            Add(net);
            net.Name += $"AutID_{Count}";
            return net;
        }
    }
}

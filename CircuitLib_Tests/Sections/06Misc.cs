using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GGL.IO;

using CircuitLib.Serialization;

namespace CircuitLib_Tests;

partial class Section
{
    public static void S06Misc()
    {
        TUtils.WriteTitle("TestMisc...");

        Tests.Run("Test Loop: ",() => {

            var oc = new Circuit();
            oc.Name = "001";



            /*
            var input0 = oc.Nodes.Create<Input>(0, 0, "Inp0");
            var input1 = oc.Nodes.Create<Input>(0, 4, "Inp1");
            var output = oc.Nodes.Create<Output>(10, 2, "Out");
            var andgate = oc.Nodes.Create<AndGate>(5, 2, "AND");
            */
            TUtils.Success("OK");
        });
    }
}

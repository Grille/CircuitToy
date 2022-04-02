using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GGL.IO;
namespace CircuitLib_Tests;

partial class Section
{
    public static void S05Serialization()
    {
        TUtils.WriteTitle("TestSerialization...");

        Tests.Run(() => {

            var oc = new Circuit();
            oc.Name = "001";
            var input0 = oc.CreateNode<Input>(0, 0, "Inp0");
            var input1 = oc.CreateNode<Input>(0, 4, "Inp1");
            var output = oc.CreateNode<Output>(10, 2, "Out");
            var andgate = oc.CreateNode<AndGate>(5, 2, "AND");

            var stream = new MemoryStream();
            var bw = new BinaryViewWriter(stream);
            var br = new BinaryViewReader(stream);

            CircuitSerialization.WriteNode(bw, oc);

            stream.Seek(0, SeekOrigin.Begin);
            var rc = (Circuit)CircuitDeserialization.ReadNode(br);

            if (rc.Name != oc.Name)
            {
                TUtils.WriteFail($"Name is {rc.Name}, expected {oc.Name}");
                return TestResult.Failure;
            }

            if (rc.Nodes.Count != oc.Nodes.Count)
            {
                TUtils.WriteFail($"Node.Count is {rc.Nodes.Count}, expected {oc.Nodes.Count}");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
    }
}

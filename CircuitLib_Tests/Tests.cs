using System;
using System.Drawing;
using System.Numerics;

using CircuitLib;
using CircuitLib.Primitives;

namespace CircuitLib_Tests;

class Tests
{
    static void Main(string[] args)
    {
        var circ = SaveFile.Load("newcircuit_2.lcp");

        TUtils.CatchExeptions = false;

        TUtils.WriteTitle("TestCascade...");
        TestCascade(true);
        TestCascade(false);

        TUtils.WriteTitle("TestPrimitives...");
        TestNode<AndGate>("00->0");
        TestNode<AndGate>("01->0");
        TestNode<AndGate>("10->0");
        TestNode<AndGate>("11->1");

        TestNode<OrGate>("00->0");
        TestNode<OrGate>("01->1");
        TestNode<OrGate>("10->1");
        TestNode<OrGate>("11->1");

        TestNode<XorGate>("00->0");
        TestNode<XorGate>("01->1");
        TestNode<XorGate>("10->1");
        TestNode<XorGate>("11->0");

        TestNode<NotGate>("0->1");
        TestNode<NotGate>("1->0");

        #region TestNetwork
        TUtils.WriteTitle("TestNetwork...");
        TestNetwork("create pins", (net) => {
            var pin0 = net.CreatePin();
            var pin1 = net.CreatePin();
            var pin2 = net.CreatePin();

            if (net.AllPins.Count != 3)
            {
                TUtils.WriteFail($"AllPin count: {net.AllPins.Count}, expected: 3");
                return TestResult.Failure;
            }
            if (net.GuardPins.Count != 3)
            {
                TUtils.WriteFail($"AllPin count: {net.AllPins.Count}, expected: 3");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        TestNetwork("connect pins", (net) => {
            var pin0 = net.CreatePin();
            var pin1 = net.CreatePin();

            net.ConnectFromTo(pin0, pin1);

            if (net.Wires.Count != 1)
            {
                TUtils.WriteFail($"Wires count: {net.AllPins.Count}, expected: 1");
                return TestResult.Failure;
            }

            var wire = net.Wires[0];
            if (wire.StartPin != pin0 || wire.EndPin != pin1)
            {
                TUtils.WriteFail($"Wires wrong connectet");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        TestNetwork("connect pin to empty point", (net) => {
            var pin0 = net.CreatePin();
            var point = new Vector2(10, 0);
            net.ConnectFromTo(pin0, point);

            if (net.AllPins.Count != 2)
            {
                TUtils.WriteFail($"AllPin count: {net.AllPins.Count}, expected: 2");
                return TestResult.Failure;
            }

            var pin1 = net.AllPins[1];
            if (net.Wires.Count != 1)
            {
                TUtils.WriteFail($"Wires count: {net.AllPins.Count}, expected: 1");
                return TestResult.Failure;
            }

            var wire = net.Wires[0];
            if (wire.StartPin != pin0 || wire.EndPin != pin1)
            {
                TUtils.WriteFail($"Wires wrong connectet");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        TestNetwork("connect pin to pin point", (net) => {
            var pin0 = net.CreatePin();
            var pin1 = net.CreatePin();
            var point = new Vector2(10, 0);
            pin1.Position = point;

            net.ConnectFromTo(pin0, point);

            if (net.AllPins.Count != 2)
            {
                TUtils.WriteFail($"AllPin count: {net.AllPins.Count}, expected: 2");
                return TestResult.Failure;
            }

            if (net.Wires.Count != 1)
            {
                TUtils.WriteFail($"Wires count: {net.AllPins.Count}, expected: 1");
                return TestResult.Failure;
            }

            var wire = net.Wires[0];
            if (wire.StartPin != pin0 || wire.EndPin != pin1)
            {
                TUtils.WriteFail($"Wires wrong connectet");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        TestNetwork("connect pin to self point", (net) => {
            var pin0 = net.CreatePin();
            var point = new Vector2(0, 0);

            net.ConnectFromTo(pin0, point);

            if (net.AllPins.Count != 1)
            {
                TUtils.WriteFail($"AllPin count: {net.AllPins.Count}, expected: 1");
                return TestResult.Failure;
            }

            if (net.Wires.Count != 0)
            {
                TUtils.WriteFail($"Wires count: {net.AllPins.Count}, expected: 0");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        #endregion

        #region TestNetworkInteraction
        TUtils.WriteTitle("TestNetworkInteraction...");
        TestCircuit("Network.Join", (c) => {
            var net0 = c.CreateNet();
            var net1 = c.CreateNet();
            var pin0 = net0.CreatePin(-1, -1);
            var pin1 = net0.CreatePin(-1, 1);
            var pin2 = net1.CreatePin(1, -1);
            var pin3 = net1.CreatePin(1, 1);

            net0.ConnectFromTo(pin0, pin1);
            net1.ConnectFromTo(pin2, pin3);

            net0.ConnectFromTo(pin1, pin2);

            if (net0.AllPins.Count != 4)
            {
                TUtils.WriteFail($"0]AllPin count: {net0.AllPins.Count}, expected: 4");
                return TestResult.Failure;
            }

            if (net0.Wires.Count != 3)
            {
                TUtils.WriteFail($"0]Wires count: {net0.Wires.Count}, expected: 3");
                return TestResult.Failure;
            }

            if (net1.AllPins.Count != 0)
            {
                TUtils.WriteFail($"1]AllPin count: {net1.AllPins.Count}, expected: 0");
                return TestResult.Failure;
            }

            if (net1.Wires.Count != 0)
            {
                TUtils.WriteFail($"1]Wires count: {net1.Wires.Count}, expected: 0");
                return TestResult.Failure;
            }

            if (c.Networks.Contains(net1))
            {
                TUtils.WriteFail($"Net1 not removed from Circuit");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        TestCircuit("Network.Split to 2, after pin.Destroy()", (c) => {
            var net0 = c.CreateNet();
            var pin0 = net0.CreatePin(0, 0);
            var pin1 = net0.CreatePin(1, 0);
            var pin2 = net0.CreatePin(2, 0);

            net0.ConnectFromTo(pin0, pin1);
            net0.ConnectFromTo(pin1, pin2);

            pin1.Destroy();

            if (net0.Wires.Count != 0)
            {
                TUtils.WriteFail($"0]Wires count: {net0.Wires.Count}, expected: 0");
                return TestResult.Failure;
            }

            if (net0.AllPins.Count != 1)
            {
                TUtils.WriteFail($"0]Pin count: {net0.AllPins.Count}, expected: 1");
                return TestResult.Failure;
            }

            if (c.Networks.Count != 2)
            {
                TUtils.WriteFail($"Network count: {c.Networks.Count}, expected: 2");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        TestCircuit("Network.Split to 2, after pin.Destroy()", (c) => {
            var net0 = c.CreateNet();
            var pin0 = net0.CreatePin(0, 0);
            var pin1 = net0.CreatePin(1, 0);
            var pin2 = net0.CreatePin(2, 0);
            var pin3 = net0.CreatePin(3, 0);
            var pin4 = net0.CreatePin(4, 0);

            net0.ConnectFromTo(pin0, pin1);
            net0.ConnectFromTo(pin1, pin2);
            net0.ConnectFromTo(pin2, pin3);
            net0.ConnectFromTo(pin3, pin4);


            pin2.Destroy();


            if (c.Networks.Count != 2)
            {
                TUtils.WriteFail($"Network count: {c.Networks.Count}, expected: 2");
                return TestResult.Failure;
            }

            var net1 = c.Networks[1];

            if (net0.AllPins.Count != 2)
            {
                TUtils.WriteFail($"0]Pin count: {net0.AllPins.Count}, expected: 2");
                return TestResult.Failure;
            }
            if (net1.AllPins.Count != 2)
            {
                TUtils.WriteFail($"1]Pin count: {net1.AllPins.Count}, expected: 2");
                return TestResult.Failure;
            }

            if (net0.Wires.Count != 1)
            {
                TUtils.WriteFail($"0]Wires count: {net0.Wires.Count}, expected: 1");
                return TestResult.Failure;
            }
            if (net1.Wires.Count != 1)
            {
                TUtils.WriteFail($"0]Wires count: {net1.Wires.Count}, expected: 1");
                return TestResult.Failure;
            }



            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });

        TestCircuit("Network.Split to 3, after pin.Destroy()", (c) => {
            var net0 = c.CreateNet();
            var pin0 = net0.CreatePin(0, 0);
            var pin1 = net0.CreatePin(1, 0);
            var pin2 = net0.CreatePin(2, 0);
            var pin3 = net0.CreatePin(3, 0);

            net0.ConnectFromTo(pin1, pin0);
            net0.ConnectFromTo(pin2, pin0);
            net0.ConnectFromTo(pin3, pin0);


            pin0.Destroy();


            if (c.Networks.Count != 3)
            {
                TUtils.WriteFail($"Network count: {c.Networks.Count}, expected: 3");
                return TestResult.Failure;
            }

            foreach (var net in c.Networks)
            {
                if (net.AllPins.Count != 1)
                {
                    TUtils.WriteFail($"Pin count: {net0.AllPins.Count}, expected: 1");
                    return TestResult.Failure;
                }
                if (net.Wires.Count != 0)
                {
                    TUtils.WriteFail($"Wires count: {net0.Wires.Count}, expected: 0");
                    return TestResult.Failure;
                }
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        TestCircuit("Network.Split after net.Disconnect(pin,pin)", (c) => {
            var net0 = c.CreateNet();
            var pin0 = net0.CreatePin(0, 0);
            var pin1 = net0.CreatePin(1, 0);
            net0.ConnectFromTo(pin0, pin1);

            net0.Disconnect(pin0, pin1);


            if (c.Networks.Count != 2)
            {
                TUtils.WriteFail($"Network count: {c.Networks.Count}, expected: 2");
                return TestResult.Failure;
            }

            foreach (var net in c.Networks)
            {
                if (net.AllPins.Count != 1)
                {
                    TUtils.WriteFail($"Pin count: {net0.AllPins.Count}, expected: 1");
                    return TestResult.Failure;
                }
                if (net.Wires.Count != 0)
                {
                    TUtils.WriteFail($"Wires count: {net0.Wires.Count}, expected: 0");
                    return TestResult.Failure;
                }
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        #endregion

        TUtils.WriteTitle("TestCircuits...");
        TestCircuit("TestAndCircuit", (c) => {

            var input0 = c.CreateNode<Input>(0, 0);
            var input1 = c.CreateNode<Input>(0, 4);
            var output = c.CreateNode<Output>(10, 2);
            var orgate = c.CreateNode<AndGate>(5, 2);

            input0.ConnectTo(orgate, 0, 0);
            input1.ConnectTo(orgate, 0, 1);
            orgate.ConnectTo(output, 0, 0);

            c.UpdateIO();

            c.InputPins[0].Active = true;
            c.InputPins[1].Active = true;

            if (c.InputPins.Length != 2)
            {
                TUtils.WriteFail($"InPin.Len != 2! {c.InputPins.Length}");
                return TestResult.Failure;
            }
            if (c.OutputPins.Length != 1)
            {
                TUtils.WriteFail($"OutPin.Len != 1! {c.OutputPins.Length}");
                return TestResult.Failure;
            }

            if (c.InputPins[0].Active != true)
            {
                TUtils.WriteFail("InPin[0] not true!");
                return TestResult.Failure;
            }
            if (c.InputPins[1].Active != true)
            {
                TUtils.WriteFail("InPin[1] not true!");
                return TestResult.Failure;
            }

            if (c.OutputPins[0].Active != true)
            {
                TUtils.WriteFail("OutPin[0] not true!");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });

        TestCircuit("TestNestedCircuit", (c) => {
            var andcirc = new Circuit();
            {
                var input0 = andcirc.CreateNode<Input>(0, 0);
                var input1 = andcirc.CreateNode<Input>(0, 4);
                var output = andcirc.CreateNode<Output>(10, 2);
                var orgate = andcirc.CreateNode<AndGate>(5, 2);
                input0.ConnectTo(orgate, 0, 0);
                input1.ConnectTo(orgate, 0, 1);
                orgate.ConnectTo(output, 0, 0);
                andcirc.UpdateIO();
            }
            {
                var input0 = c.CreateNode<Input>(0, 0);
                var input1 = c.CreateNode<Input>(0, 4);
                var output = c.CreateNode<Output>(10, 2);
                var orgate = andcirc;
                c.AddNode(orgate);

                input0.ConnectTo(orgate, 0, 0);
                input1.ConnectTo(orgate, 0, 1);
                orgate.ConnectTo(output, 0, 0);

                c.UpdateIO();

                if (c.InputPins.Length != 2)
                {
                    TUtils.WriteFail($"InPin.Len != 2! {c.InputPins.Length}");
                    return TestResult.Failure;
                }
                if (c.OutputPins.Length != 1)
                {
                    TUtils.WriteFail($"OutPin.Len != 1! {c.OutputPins.Length}");
                    return TestResult.Failure;
                }

                c.InputPins[0].Active = true;
                c.InputPins[1].Active = true;

                if (c.OutputPins[0].Active != true)
                {
                    TUtils.WriteFail("OutPin[0] not true!");
                    return TestResult.Failure;
                }
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });

        TUtils.WriteResults();
    }

    static void TestCascade(bool expectet)
    {
        TUtils.Write($"Cascade [Exit -> Net -> Entry] to ({expectet}): ");
        TUtils.Test(() => {
            var circuit = new Circuit();
            var exit = new OutputPin(circuit);
            var entry = new InputPin(circuit);
            var network = new Network();

            network.Add(exit);
            network.Add(entry);

            exit.Active = network.Active = entry.Active = !expectet;

            if (exit.Active == expectet || network.Active == expectet || entry.Active == expectet)
            {
                TUtils.WriteFail($"Reset Failed [Exit<{exit.Active}> -> Net<{network.Active}> -> Entry<{entry.Active}>]");
                return TestResult.Failure;
            }

            exit.Active = expectet;

            if (entry.Active == expectet)
            {
                TUtils.WriteSucces("OK");
                return TestResult.Success;
            }
            else
            {
                TUtils.WriteFail($"Cascade Failed [Exit<{exit.Active}> -> Net<{network.Active}> -> Entry<{entry.Active}>]");
                return TestResult.Failure;
            }
        });
    }

    static void TestNode<T>(string inp) where T : Node, new()
    {
        var split = inp.Split("->");
        var instr = split[0];
        var outstr = split[1];
        var input = TUtils.StrToBoolArray(split[0]);
        var output = TUtils.StrToBoolArray(split[1]);

        TUtils.Write($"Test {typeof(T).Name} [{instr}->{outstr}]: ");
        TUtils.Test(() => {
            var node = new T();

            if (input.Length != node.InputPins.Length)
            {
                TUtils.WriteFail($"In count: {node.InputPins.Length} Expected: {input.Length}");
                return TestResult.Failure;
            }

            if (output.Length != node.OutputPins.Length)
            {
                TUtils.WriteFail($"Out count: {node.OutputPins.Length} Expected: {output.Length}");
                return TestResult.Failure;
            }

            for (int i = 0; i < input.Length; i++)
            {
                node.InputPins[i].Active = input[i];
            }

            node.Update();

            bool failed = false;
            for (int i = 0; i < output.Length; i++)
            {
                if (node.OutputPins[i].Active != output[i])
                {
                    TUtils.WriteFail($"Output at [{i}] is {node.OutputPins[i].Active}");
                    failed = true;
                }
            }

            if (failed)
            {
                return TestResult.Failure;
            }
            else
            {
                TUtils.WriteSucces("OK");
                return TestResult.Success;
            }

        });
    }

    static void TextCircuit(Circuit circuit, string inp)
    {

    }

    static void TestNetwork(string title, Func<Network, TestResult> test)
    {
        TUtils.Write($"Test Network <{title}>: ");
        TUtils.Test(() => {
            var circuit = new Circuit();
            var net = circuit.CreateNet();
            return test(net);
        });
    }

    static void TestCircuit(string title, Func<Circuit, TestResult> test)
    {
        TUtils.Write($"Test Circuit <{title}>: ");
        TUtils.Test(() => {
            var circuit = new Circuit();
            return test(circuit);
        });
    }
}


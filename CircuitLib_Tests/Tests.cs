using System;
using CircuitLib;
using CircuitLib.Primitives;

namespace CircuitLib_Tests;

class Tests
{
    static void Main(string[] args)
    {
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

        TUtils.WriteResults();
    }

    static void TestCascade(bool expectet)
    {
        TUtils.Write($"Cascade [Exit -> Net -> Entry] to ({expectet}): ");
        TUtils.Test(() => {
            var exit = new OutputPin();
            var entry = new InputPin();
            var network = new Network();

            network.Connect(exit);
            network.Connect(entry);

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
        var input = TUtils.StrToBoolArray(split[0]);
        var output = TUtils.StrToBoolArray(split[1]);

        TestNode<T>(input, output);

    }
    static void TestNode<T>(bool[] input, bool[] output) where T : Node, new()
    {
        string instr = TUtils.BoolArrayToStr(input);
        string outstr = TUtils.BoolArrayToStr(output);

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
            for (int i = 0;i < output.Length; i++)
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
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib_Tests;

class Tests
{
    public static void Run(Func<TestResult> test)
    {
        TestResult result;
        if (TUtils.CatchExeptions)
        {
            try
            {
                result = test();
            }
            catch (Exception e)
            {
                TUtils.WriteFail($"Error\n");
                TUtils.WriteError($"{e}");
                result = TestResult.Error;

            }
        }
        else
        {
            result = test();
        }
        TUtils.Write("\n");

        switch (result)
        {
            case TestResult.Success:
                TUtils.SuccessCount++;
                break;
            case TestResult.Failure:
                TUtils.FailureCount++;
                break;
            case TestResult.Error:
                TUtils.ErrorCount++;
                break;
        }
    }

    public static void RunCascade(bool expectet)
    {
        TUtils.Write($"Cascade [Exit -> Net -> Entry] to ({expectet}): ");
        Tests.Run(() => {
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

    public static void RunIOTable<T>(string inp) where T : Node, new()
    {
        var split = inp.Split("->");
        var instr = split[0];
        var outstr = split[1];
        var input = TUtils.StrToBoolArray(split[0]);
        var output = TUtils.StrToBoolArray(split[1]);

        TUtils.Write($"Test {typeof(T).Name} [{instr}->{outstr}]: ");
        Run(() => {
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

    public static void RunNetwork(string title, Func<Network, TestResult> test)
    {
        TUtils.Write($"Test Network <{title}>: ");
        Run(() => {
            var circuit = new Circuit();
            var net = circuit.CreateNet();
            return test(net);
        });
    }

    public static void RunCircuit(Circuit circuit, string inp)
    {

    }



    public static void RunCircuit(string title, Func<Circuit, TestResult> test)
    {
        TUtils.Write($"Test Circuit <{title}>: ");
        Tests.Run(() => {
            var circuit = new Circuit();
            return test(circuit);
        });
    }
}

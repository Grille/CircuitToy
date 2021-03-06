using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib_Tests;

class Tests
{
    public static void Run(string msg, Func<TestResult> test)
    {
        TUtils.Write(msg);
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

    public static void RunCascade(State reset, State send, State expected)
    {
        string name = $"Cascade [Exit -> Net -> Entry] ({reset}) -> ({send}) = {expected}: ";
        Run(name, () => {
            var circuit = new Circuit();
            var exit = new OutputPin(circuit);
            var entry = new InputPin(circuit);
            var network = new Network();
            network.Owner = circuit;

            network.BeginEdit();
            network.Pins.Add(exit);
            network.Pins.Add(entry);
            network.ConnectFromTo(exit, entry);
            network.EndEdit();

            exit.State = network.State = entry.State = reset;

            if (exit.State != reset || network.State != reset || entry.State != reset)
            {
                TUtils.WriteFail($"Reset Failed [Exit<{exit.State}> -> Net<{network.State}> -> Entry<{entry.State}>]");
                return TestResult.Failure;
            }

            exit.State = send;
            exit.ConnectedNetwork.Update();
            exit.ConnectedNetwork.WaitIdle();

            if (entry.State == expected)
            {
                TUtils.WriteSucces("OK");
                return TestResult.Success;
            }
            else
            {
                TUtils.WriteFail($"Cascade Failed [Exit<{exit.State}> -> Net<{network.State}> -> Entry<{entry.State}>]");
                return TestResult.Failure;
            }
        });
    }

    public static void RunIOTable<T>(string inp) where T : Node, new()
    {
        var split = inp.Split("->");
        var instr = split[0];
        var outstr = split[1];
        var input = TUtils.StrToStateArray(split[0]);
        var output = TUtils.StrToStateArray(split[1]);

        string name = $"Test {typeof(T).Name} [{instr}->{outstr}]: ";
        Run(name, () => {
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
                node.InputPins[i].State = input[i];
            }

            node.Update();
            node.WaitIdle();

            bool failed = false;
            for (int i = 0; i < output.Length; i++)
            {
                if (node.OutputPins[i].State != output[i])
                {
                    TUtils.WriteFail($"Output at [{i}] is {node.OutputPins[i].State}");
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
        string name = $"Test Network <{title}>: ";
        Run(name, () => {
            var circuit = new Circuit();
            var net = circuit.Networks.Create();
            return test(net);
        });
    }

    public static void RunCircuit(Circuit circuit, string inp)
    {

    }



    public static void RunCircuit(string title, Func<Circuit, TestResult> test)
    {
        string name = $"Test Circuit <{title}>: ";
        Run(name, () => {
            var circuit = new Circuit();
            circuit.Name = "main";
            return test(circuit);
        });
    }
}

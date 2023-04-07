using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib_Tests;

class Tests
{
    public static void Run(string msg, Action test)
    {
        TUtils.Write(msg);
        TestResult result = TestResult.None;

        try
        {
            test();
        }
        catch (SuccsessException e)
        {
            TUtils.WriteSuccess(e.Message);
            result = TestResult.Success;
        }
        catch (FailException e)
        {
            TUtils.WriteFail(e.Message);
            result = TestResult.Failure;
        }
        catch (Exception e)
        {
            TUtils.WriteFail($"Error\n");
            TUtils.WriteError($"{e}");
            result = TestResult.Error;
        }

        TUtils.Write("\n");

        switch (result)
        {
            case TestResult.None:
                throw new InvalidOperationException();
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
                TUtils.Fail($"Reset Failed [Exit<{exit.State}> -> Net<{network.State}> -> Entry<{entry.State}>]");
            }

            exit.State = send;
            exit.ConnectedNetwork.Update();
            exit.ConnectedNetwork.WaitIdle();

            if (entry.State == expected)
            {
                TUtils.Success("OK");
            }
            else
            {
                TUtils.Fail($"Cascade Failed [Exit<{exit.State}> -> Net<{network.State}> -> Entry<{entry.State}>]");
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
                TUtils.Fail($"In count: {node.InputPins.Length} Expected: {input.Length}");
            }

            if (output.Length != node.OutputPins.Length)
            {
                TUtils.Fail($"Out count: {node.OutputPins.Length} Expected: {output.Length}");
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
                    TUtils.WriteFail($"Out[{i}]=={node.OutputPins[i].State} ");
                    failed = true;
                }
            }

            if (failed)
            {
                TUtils.Fail("FAIL");
            }
            else
            {
                TUtils.Success("OK");
            }

        });
    }

    public static void RunNetwork(string title, Action<Network> test)
    {
        string name = $"Test Network <{title}>: ";
        Run(name, () => {
            var circuit = new Circuit();
            var net = circuit.Networks.Create();
            test(net);
        });
    }

    public static void RunCircuit(Circuit circuit, string inp)
    {

    }



    public static void RunCircuit(string title, Action<Circuit> test)
    {
        string name = $"Test Circuit <{title}>: ";
        Run(name, () => {
            var circuit = new Circuit();
            circuit.Name = "main";
            test(circuit);
        });
    }
}

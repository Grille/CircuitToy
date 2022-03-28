namespace CircuitLib_Tests;

partial class Section
{
    public static void S04Circuits()
    {
        TUtils.WriteTitle("TestCircuits...");


        Tests.RunCircuit("AND Circuit", (c) => {

            var input0 = c.CreateNode<Input>(0, 0);
            var input1 = c.CreateNode<Input>(0, 4);
            var output = c.CreateNode<Output>(10, 2, "out");
            var andgate = c.CreateNode<AndGate>(5, 2, "and");

            input0.ConnectTo(andgate, 0, 0);
            input1.ConnectTo(andgate, 0, 1);
            andgate.ConnectTo(output, 0, 0);

            var innet0 = input0.OutputPins[0].ConnectedNetwork;
            var innet1 = input1.OutputPins[0].ConnectedNetwork;
            innet0.Name = "innet0";
            innet1.Name = "innet1";

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

            c.InputPins[0].State = State.High;
            c.InputPins[1].State = State.High;
            c.Update();
            c.WaitIdle();

            if (TUtils.AssertPinState(c.InputPins[0], State.High))
                return TestResult.Failure;

            if (TUtils.AssertPinState(c.InputPins[1], State.High))
                return TestResult.Failure;

            if (TUtils.AssertNetState(innet0, State.High))
                return TestResult.Failure;

            if (TUtils.AssertNetState(innet1, State.High))
                return TestResult.Failure;

            if (TUtils.AssertPinState(andgate.InputPins[0], State.High))
                return TestResult.Failure;

            if (TUtils.AssertPinState(andgate.InputPins[1], State.High))
                return TestResult.Failure;

            if (TUtils.AssertPinState(andgate.OutputPins[0], State.High))
                return TestResult.Failure;

            if (TUtils.AssertPinState(output.InputPins[0], State.High))
                return TestResult.Failure;

            if (TUtils.AssertPinState(c.OutputPins[0], State.High))
                return TestResult.Failure;

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });


        Tests.RunCircuit("Nested AND Circuit", (c) => {
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
                c.AddNode(andcirc);

                input0.ConnectTo(andcirc, 0, 0);
                input1.ConnectTo(andcirc, 0, 1);
                andcirc.ConnectTo(output, 0, 0);

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

                c.InputPins[0].State = State.High;
                c.InputPins[1].State = State.High;
                c.Update();
                c.WaitIdle();

                if (TUtils.AssertPinState(c.OutputPins[0], State.High))
                    return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });


        Tests.RunCircuit("SR Latch", (c) => {

            var input0set = c.CreateNode<Input>(0, 0);
            var input1reset = c.CreateNode<Input>(0, 4);
            var output = c.CreateNode<Output>(10, 2);

            var norgate0 = c.CreateNode<NOrGate>(5, -5, "nor0");
            var norgate1 = c.CreateNode<NOrGate>(5, 5, "nor1");


            input0set.ConnectTo(norgate0, 0, 0);

            input1reset.ConnectTo(norgate1, 0, 1);

            norgate0.ConnectTo(norgate1, 0, 0);

            norgate1.ConnectTo(norgate0, 0, 1);
            norgate1.ConnectTo(output, 0, 0);

            c.UpdateIO();

            c.InputPins[0].State = State.High;
            c.InputPins[1].State = State.Low;
            c.Update();
            c.WaitIdle();

            if (TUtils.AssertPinState(c.InputPins[0], State.High))
                return TestResult.Failure;

            if (TUtils.AssertPinState(c.InputPins[1], State.Low))
                return TestResult.Failure;

            if (TUtils.AssertPinState(norgate0.InputPins[0], State.High))
                return TestResult.Failure;

            if (TUtils.AssertPinState(norgate1.InputPins[0], State.Low))
                return TestResult.Failure;

            if (TUtils.AssertPinState(c.OutputPins[0], State.High))
                return TestResult.Failure;

            c.InputPins[0].State = State.Low;
            c.Update();
            c.WaitIdle();

            if (TUtils.AssertPinState(c.OutputPins[0], State.High))
                return TestResult.Failure;

            c.InputPins[1].State = State.High;
            c.Update();
            c.WaitIdle();
            c.InputPins[1].State = State.Low;
            c.Update();
            c.WaitIdle();

            if (TUtils.AssertPinState(c.OutputPins[0], State.Low))
                return TestResult.Failure;

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
    }
}

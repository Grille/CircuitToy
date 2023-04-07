namespace CircuitLib_Tests;

partial class Section
{
    public static void S04Circuits()
    {
        TUtils.WriteTitle("TestCircuits...");


        Tests.RunCircuit("AND Circuit", (c) => {

            var input0 = c.Nodes.Create<Input>(0, 0, "Inp0");
            var input1 = c.Nodes.Create<Input>(0, 4, "Inp1");
            var output = c.Nodes.Create<Output>(10, 2, "Out");
            var andgate = c.Nodes.Create<AndGate>(5, 2, "AND");

            input0.ConnectTo(andgate, 0, 0);
            input1.ConnectTo(andgate, 0, 1);
            andgate.ConnectTo(output, 0, 0);

            var innet0 = input0.OutputPins[0].ConnectedNetwork;
            var innet1 = input1.OutputPins[0].ConnectedNetwork;
            innet0.Name = "InNet0";
            innet1.Name = "InNet1";

            c.UpdateIO();

            TUtils.AssertValue(c.InputPins.Length, 2, "InputPins.Length");
            TUtils.AssertValue(c.OutputPins.Length, 1, "OutputPins.Length");

            c.InputPins[0].State = State.High;
            c.InputPins[1].State = State.High;
            c.Update();
            c.WaitIdle();

            TUtils.AssertPinState(c.InputPins[0], State.High);
            TUtils.AssertPinState(c.InputPins[1], State.High);
            TUtils.AssertPinState(input0.OutputPins[0], State.High);
            TUtils.AssertPinState(input1.OutputPins[0], State.High);
            TUtils.AssertNetState(innet0, State.High);
            TUtils.AssertNetState(innet1, State.High);
            TUtils.AssertPinState(andgate.InputPins[0], State.High);
            TUtils.AssertPinState(andgate.InputPins[1], State.High);
            TUtils.AssertPinState(andgate.OutputPins[0], State.High);
            TUtils.AssertPinState(output.InputPins[0], State.High);
            TUtils.AssertPinState(c.OutputPins[0], State.High);
            TUtils.Success("OK");
        });


        Tests.RunCircuit("Nested AND Circuit", (c) => {
            var andcirc = new Circuit();
            {
                andcirc.Name = "AND_Wrapper";
                var input0 = andcirc.Nodes.Create<Input>(0, 0);
                var input1 = andcirc.Nodes.Create<Input>(0, 4);
                var output = andcirc.Nodes.Create<Output>(10, 2);
                var andgate = andcirc.Nodes.Create<AndGate>(5, 2, "AND");
                input0.ConnectTo(andgate, 0, 0);
                input1.ConnectTo(andgate, 0, 1);
                andgate.ConnectTo(output, 0, 0);
                andcirc.UpdateIO();
            }
            {
                var input0 = c.Nodes.Create<Input>(0, 0);
                var input1 = c.Nodes.Create<Input>(0, 4);
                var output = c.Nodes.Create<Output>(10, 2);
                c.Nodes.Add(andcirc);

                input0.ConnectTo(andcirc, 0, 0);
                input1.ConnectTo(andcirc, 0, 1);
                andcirc.ConnectTo(output, 0, 0);

                c.UpdateIO();

                TUtils.AssertValue(c.InputPins.Length, 2, "InputPins.Length");
                TUtils.AssertValue(c.OutputPins.Length, 1, "OutputPins.Length");

                c.InputPins[0].State = State.High;
                c.InputPins[1].State = State.High;
                c.Update();
                c.WaitIdle();

                TUtils.AssertPinState(andcirc.OutputPins[0], State.High);
                TUtils.AssertPinState(c.OutputPins[0], State.High);
            }

            TUtils.Success("OK");
        });


        Tests.RunCircuit("SR Latch", (c) => {

            var input0set = c.Nodes.Create<Input>(0, 0);
            var input1reset = c.Nodes.Create<Input>(0, 4);
            var output = c.Nodes.Create<Output>(10, 2);

            var norgate0 = c.Nodes.Create<NOrGate>(5, -5, "nor0");
            var norgate1 = c.Nodes.Create<NOrGate>(5, 5, "nor1");


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

            TUtils.AssertPinState(c.InputPins[0], State.High);
            TUtils.AssertPinState(c.InputPins[1], State.Low);
            TUtils.AssertPinState(norgate0.InputPins[0], State.High);
            TUtils.AssertPinState(norgate1.InputPins[0], State.Low);
            TUtils.AssertPinState(c.OutputPins[0], State.High);

            c.InputPins[0].State = State.Low;
            c.Update();
            c.WaitIdle();

            TUtils.AssertPinState(c.OutputPins[0], State.High);

            c.InputPins[1].State = State.High;
            c.Update();
            c.WaitIdle();
            c.InputPins[1].State = State.Low;
            c.Update();
            c.WaitIdle();

            TUtils.AssertPinState(c.OutputPins[0], State.Low);

            TUtils.Success("OK");
        });
    }
}

namespace CircuitLib_Tests;

partial class Section
{
    public static void S03NetworkInteraction()
    {
        TUtils.WriteTitle("TestNetworkInteraction...");
        Tests.RunCircuit("Network.Join", (c) => {
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
        Tests.RunCircuit("Network.Split to 2, after pin.Destroy()", (c) => {
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
        Tests.RunCircuit("Network.Split to 2, after pin.Destroy()", (c) => {
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

        Tests.RunCircuit("Network.Split to 3, after pin.Destroy()", (c) => {
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
        Tests.RunCircuit("Network.Split after net.Disconnect(pin,pin)", (c) => {
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

        Tests.RunCircuit("Connect 1 In to 2 Out", (c) => {
            var input0 = c.CreateNode<Input>(0, 4);
            var output0 = c.CreateNode<Output>(10, 2);
            var output1 = c.CreateNode<Output>(10, -2);

            input0.ConnectTo(output0, 0, 0);
            input0.ConnectTo(output1, 0, 0);


            if (c.Networks.Count != 1)
            {
                TUtils.WriteFail($"Network count: {c.Networks.Count}, expected: 1");
                return TestResult.Failure;
            }

            var net = c.Networks[0];

            if (net.AllPins.Count != 3)
            {
                TUtils.WriteFail($"Pin count: {net.AllPins.Count}, expected: 3");
                return TestResult.Failure;
            }
            if (net.Wires.Count != 2)
            {
                TUtils.WriteFail($"Wires count: {net.Wires.Count}, expected: 2");
                return TestResult.Failure;
            }


            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });

        Tests.RunCircuit("Connect 2 Out to 1 In", (c) => {
            var input0 = c.CreateNode<Input>(0, 4);
            var output0 = c.CreateNode<Output>(10, 2);
            var output1 = c.CreateNode<Output>(10, -2);

            output0.InputPins[0].ConnectTo(input0.OutputPins[0]);
            output1.InputPins[0].ConnectTo(input0.OutputPins[0]);


            if (c.Networks.Count != 1)
            {
                TUtils.WriteFail($"Network count: {c.Networks.Count}, expected: 1");
                return TestResult.Failure;
            }

            var net = c.Networks[0];

            if (net.AllPins.Count != 3)
            {
                TUtils.WriteFail($"Pin count: {net.AllPins.Count}, expected: 3");
                return TestResult.Failure;
            }
            if (net.Wires.Count != 2)
            {
                TUtils.WriteFail($"Wires count: {net.Wires.Count}, expected: 2");
                return TestResult.Failure;
            }


            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
    }
}

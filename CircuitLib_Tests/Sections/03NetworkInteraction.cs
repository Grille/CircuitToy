namespace CircuitLib_Tests;

partial class Section
{
    public static void S03NetworkInteraction()
    {
        TUtils.WriteTitle("TestNetworkInteraction...");
        Tests.RunCircuit("Network.Join", (c) => {
            var net0 = c.Networks.Create();
            var net1 = c.Networks.Create();

            net0.BeginEdit();
            net1.BeginEdit();

            var pin0 = net0.Pins.Create(-1, -1);
            var pin1 = net0.Pins.Create(-1, 1);
            var pin2 = net1.Pins.Create(1, -1);
            var pin3 = net1.Pins.Create(1, 1);

            net0.ConnectFromTo(pin0, pin1);
            net1.ConnectFromTo(pin2, pin3);

            net0.ConnectFromTo(pin1, pin2);

            net0.EndEdit();
            net1.EndEdit();

            if (TUtils.AssertListCount(net0.Pins, 4))
                return TestResult.Failure;

            if (TUtils.AssertListCount(net0.Wires, 3))
                return TestResult.Failure;

            if (TUtils.AssertListCount(net1.Pins, 0))
                return TestResult.Failure;

            if (TUtils.AssertListCount(net1.Wires, 0))
                return TestResult.Failure;

            if (c.Networks.Contains(net1))
            {
                TUtils.WriteFail($"Net1 not removed from Circuit");
                return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        Tests.RunCircuit("Network.Split to 2, after pin.Destroy()", (c) => {
            var net0 = c.Networks.Create();

            net0.BeginEdit();

            var pin0 = net0.Pins.Create(0, 0);
            var pin1 = net0.Pins.Create(1, 0);
            var pin2 = net0.Pins.Create(2, 0);

            net0.ConnectFromTo(pin0, pin1);
            net0.ConnectFromTo(pin1, pin2);

            net0.EndEdit();

            pin1.Destroy();

            net0.Cleanup();

            if (TUtils.AssertListCount(c.Networks, 2))
                return TestResult.Failure;

            foreach (var net in c.Networks)
            {
                if (TUtils.AssertListCount(net.Pins, 1))
                    return TestResult.Failure;

                if (TUtils.AssertListCount(net.Wires, 0))
                    return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        Tests.RunCircuit("Network.Split to 2, after pin.Destroy()", (c) => {
            var net0 = c.Networks.Create();

            net0.BeginEdit();

            var pin0 = net0.Pins.Create(0, 0);
            var pin1 = net0.Pins.Create(1, 0);
            var pin2 = net0.Pins.Create(2, 0);
            var pin3 = net0.Pins.Create(3, 0);
            var pin4 = net0.Pins.Create(4, 0);

            net0.ConnectFromTo(pin0, pin1);
            net0.ConnectFromTo(pin1, pin2);
            net0.ConnectFromTo(pin2, pin3);
            net0.ConnectFromTo(pin3, pin4);

            net0.EndEdit();

            pin2.Destroy();

            if (TUtils.AssertListCount(c.Networks, 2))
                return TestResult.Failure;

            var net1 = c.Networks[1];

            if (TUtils.AssertListCount(net0.Pins, 2))
                return TestResult.Failure;

            if (TUtils.AssertListCount(net1.Pins, 2))
                return TestResult.Failure;


            if (TUtils.AssertListCount(net0.Wires, 1))
                return TestResult.Failure;

            if (TUtils.AssertListCount(net1.Wires, 1))
                return TestResult.Failure;

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });

        Tests.RunCircuit("Network.Split to 3, after pin.Destroy()", (c) => {
            var net0 = c.Networks.Create();

            net0.BeginEdit();

            var pin0 = net0.Pins.Create(0, 0);
            var pin1 = net0.Pins.Create(1, 0);
            var pin2 = net0.Pins.Create(2, 0);
            var pin3 = net0.Pins.Create(3, 0);

            net0.ConnectFromTo(pin1, pin0);
            net0.ConnectFromTo(pin2, pin0);
            net0.ConnectFromTo(pin3, pin0);

            net0.EndEdit();

            pin0.Destroy();

            if (TUtils.AssertListCount(c.Networks, 3))
                return TestResult.Failure;

            foreach (var net in c.Networks)
            {
                if (TUtils.AssertListCount(net.Pins, 1))
                    return TestResult.Failure;

                if (TUtils.AssertListCount(net.Wires, 0))
                    return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        Tests.RunCircuit("Network.Split to 2, after wire.Destroy()", (c) => {
            var net0 = c.Networks.Create();

            net0.BeginEdit();

            var pin0 = net0.Pins.Create(0, 0);
            var pin1 = net0.Pins.Create(1, 0);

            net0.ConnectFromTo(pin0, pin1);

            net0.EndEdit();

            var wire = pin0.ConnectedWires[0];

            wire.Destroy();


            if (TUtils.AssertListCount(c.Networks, 2))
                return TestResult.Failure;

            foreach (var net in c.Networks)
            {
                if (TUtils.AssertListCount(net.Pins, 1))
                    return TestResult.Failure;

                if (TUtils.AssertListCount(net.Wires, 0))
                    return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });
        Tests.RunCircuit("Network.Split after net.Disconnect(pin,pin)", (c) => {
            var net0 = c.Networks.Create();

            net0.BeginEdit();

            var pin0 = net0.Pins.Create(0, 0);
            var pin1 = net0.Pins.Create(1, 0);
            net0.ConnectFromTo(pin0, pin1);

            net0.EndEdit();

            net0.Disconnect(pin0, pin1);

            if (TUtils.AssertListCount(c.Networks, 2))
                return TestResult.Failure;

            foreach (var net in c.Networks)
            {
                if (TUtils.AssertListCount(net.Pins, 1))
                    return TestResult.Failure;

                if (TUtils.AssertListCount(net.Wires, 0))
                    return TestResult.Failure;
            }

            TUtils.WriteSucces("OK");
            return TestResult.Success;
        });

        Tests.RunCircuit("Connect 1 In to 2 Out", (c) => {
            var input0 = c.Nodes.Create<Input>(0, 4);
            var output0 = c.Nodes.Create<Output>(10, 2);
            var output1 = c.Nodes.Create<Output>(10, -2);

            input0.ConnectTo(output0, 0, 0);
            input0.ConnectTo(output1, 0, 0);

            if (c.Networks.Count != 1)
            {
                TUtils.WriteFail($"Network count: {c.Networks.Count}, expected: 1");
                return TestResult.Failure;
            }

            var net = c.Networks[0];

            if (net.Pins.Count != 3)
            {
                TUtils.WriteFail($"Pin count: {net.Pins.Count}, expected: 3");
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
            var input0 = c.Nodes.Create<Input>(0, 4);
            var output0 = c.Nodes.Create<Output>(10, 2);
            var output1 = c.Nodes.Create<Output>(10, -2);

            output0.InputPins[0].ConnectTo(input0.OutputPins[0]);
            output1.InputPins[0].ConnectTo(input0.OutputPins[0]);


            if (c.Networks.Count != 1)
            {
                TUtils.WriteFail($"Network count: {c.Networks.Count}, expected: 1");
                return TestResult.Failure;
            }

            var net = c.Networks[0];

            if (net.Pins.Count != 3)
            {
                TUtils.WriteFail($"Pin count: {net.Pins.Count}, expected: 3");
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

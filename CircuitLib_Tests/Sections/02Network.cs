namespace CircuitLib_Tests;

partial class Section
{
    public static void S02Network()
    {
        TUtils.WriteTitle("TestNetwork...");
        Tests.RunNetwork("create pins", (net) => {
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
        Tests.RunNetwork("connect pins", (net) => {
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
        Tests.RunNetwork("connect pin to empty position", (net) => {
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
        Tests.RunNetwork("connect pin to pin-occupied position", (net) => {
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
        Tests.RunNetwork("connect pin to own position", (net) => {
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
        Tests.RunNetwork("connect pin to own position", (net) => {
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
    }
}

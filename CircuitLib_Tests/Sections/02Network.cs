namespace CircuitLib_Tests;

partial class Section
{
    public static void S02Network()
    {
        TUtils.WriteTitle("TestNetwork...");
        Tests.RunNetwork("create pins", (net) => {
            net.BeginEdit();

            var pin0 = net.Pins.Create();
            var pin1 = net.Pins.Create();
            var pin2 = net.Pins.Create();

            TUtils.AssertValue(net.Pins.Count, 3, "Pins.Count");
            TUtils.AssertValue(net.Pins.NetPins.Count, 3, "NetPins.Count");

            TUtils.Success("OK");
        });
        Tests.RunNetwork("connect pins", (net) => {
            net.BeginEdit();

            var pin0 = net.Pins.Create();
            var pin1 = net.Pins.Create();

            net.ConnectFromTo(pin0, pin1);

            TUtils.AssertValue(net.Wires.Count, 1, "Wires.Count");

            var wire = net.Wires[0];
            if (wire.StartPin != pin0 || wire.EndPin != pin1)
            {
                TUtils.Fail($"Wires wrong connectet");
            }

            TUtils.Success("OK");
        });
        Tests.RunNetwork("connect pin to empty position", (net) => {
            net.BeginEdit();

            var pin0 = net.Pins.Create();
            var point = new Vector2(10, 0);
            net.ConnectFromTo(pin0, point);

            TUtils.AssertValue(net.Pins.Count, 2, "Pins.Count");
            TUtils.AssertValue(net.Wires.Count, 1, "Wires.Count");

            var pin1 = net.Pins[1];
            var wire = net.Wires[0];
            if (wire.StartPin != pin0 || wire.EndPin != pin1)
            {
                TUtils.Fail($"Wires wrong connectet");
            }

            TUtils.Success("OK");
        });
        Tests.RunNetwork("connect pin to pin-occupied position", (net) => {
            net.BeginEdit();

            var pin0 = net.Pins.Create();
            var pin1 = net.Pins.Create();
            var point = new Vector2(10, 0);
            pin1.Position = point;

            net.ConnectFromTo(pin0, point);

            TUtils.AssertValue(net.Pins.Count, 2, "Pins.Count");
            TUtils.AssertValue(net.Wires.Count, 1, "Wires.Count");

            var wire = net.Wires[0];
            if (wire.StartPin != pin0 || wire.EndPin != pin1)
            {
                TUtils.Fail($"Wires wrong connectet");
            }

            TUtils.Success("OK");
        });
        Tests.RunNetwork("connect pin to own position", (net) => {
            net.BeginEdit();

            var pin0 = net.Pins.Create();
            var point = new Vector2(0, 0);

            net.ConnectFromTo(pin0, point);

            TUtils.AssertValue(net.Pins.Count, 1, "Pins.Count");
            TUtils.AssertValue(net.Wires.Count, 0, "Wire.Count");

            TUtils.Success("OK");
        });
        Tests.RunNetwork("connect pin to own position", (net) => {
            net.BeginEdit();

            var pin0 = net.Pins.Create();
            var point = new Vector2(0, 0);

            net.ConnectFromTo(pin0, point);

            TUtils.AssertValue(net.Pins.Count, 1, "Pins.Count");
            TUtils.AssertValue(net.Wires.Count, 0, "Wires.Count");

            TUtils.Success("OK");
        });
    }
}

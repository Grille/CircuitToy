namespace CircuitLib_Tests;

static class TUtils
{
    public static int SuccessCount = 0;
    public static int FailureCount = 0;
    public static int ErrorCount = 0;

    public static bool CatchExeptions = false;


    public static void Write(string msg, ConsoleColor color)
    {
        var bcolor = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.Write(msg);

        Console.ForegroundColor = bcolor;
    }

    public static void Write(string msg)
    {
        Console.Write(msg);
    }
    public static void WriteTitle(string msg)
    {
        Write($"\n{msg}\n", ConsoleColor.Cyan);
    }
    public static void WriteSucces(string msg)
    {
        Write(msg, ConsoleColor.Green);
    }
    public static void WriteFail(string msg)
    {
        Write(msg, ConsoleColor.Magenta);
    }
    public static void WriteError(string msg)
    {
        Write(msg, ConsoleColor.Red);
    }
    public static void WriteResults()
    {
        WriteTitle("Results:");
        float testCount = SuccessCount + ErrorCount + FailureCount;
        float sucCount = SuccessCount;
        float failCount = FailureCount + ErrorCount;
        Write($"Testcases: {testCount}\n");
        Write($"* Success: {sucCount} {(int)(sucCount / testCount * 100)}%\n");
        Write($"* failure: {failCount} {(int)(failCount / testCount * 100)}%\n");
    }
    public static void WriteList<T>(List<T> list, Func<T, string> func)
    {
        var bcolor = Console.BackgroundColor;
        var fcolor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("[");
        for (int i = 0; i < list.Count; i++)
        {
            string value = func(list[i]);
            if (value == "")
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.Write(" ");
            }
            else if (value == "\n" || value == "\r" || value == "\r\n")
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.Write("\\");
            }
            else
            {
                if (i % 2 == 0)
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                else
                    Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write(value);
            }
        }

        Console.BackgroundColor = bcolor;
        Console.Write($"]:{list.Count}");
        Console.ForegroundColor = fcolor;
    }

    public static bool[] StrToBoolArray(string str)
    {
        string tinstr = str.Trim();

        bool[] outarray = new bool[tinstr.Length];

        for (int i = 0; i < tinstr.Length; i++)
            outarray[i] = tinstr[i] == '1';

        return outarray;
    }

    public static string BoolArrayToStr(bool[] array)
    {
        string outstr = "";
        for (int i = 0; i < array.Length; i++)
            outstr += array[i] ? "1" : "0";

        return outstr;
    }

    public static State[] StrToStateArray(string str)
    {
        string tinstr = str.Trim();

        State[] outarray = new State[tinstr.Length];

        for (int i = 0; i < tinstr.Length; i++)
        {
            outarray[i] = tinstr[i] switch {
                '0' => State.Low,
                '1' => State.High,
                'Z' => State.Off,
                _ => State.Error,
            };
        }

        return outarray;
    }

    public static bool AssertPinState(IOPin pin, State state, string msg = "")
    {
        string pinType;
        if (pin is InputPin)
            pinType = $"{pin.Owner.Name}.InPin[{Array.IndexOf(pin.Owner.InputPins, pin)}]";
        else
            pinType = $"{pin.Owner.Name}.OutPin[{Array.IndexOf(pin.Owner.OutputPins, pin)}]";

        if (pin.State != state)
        {
            WriteFail($"{msg}{pinType} is {pin.State}, expected {state}");
            return true;
        }
        return false;
    }

    public static bool AssertNetState(Network net, State state, string msg = "")
    {
        if (net.State != state)
        {
            WriteFail($"{msg}{net.Name} is {net.State}, expected {state}");
            return true;
        }
        return false;
    }
}


namespace CircuitLib_Tests;

partial class Section
{
    public static void S00Cascade()
    {
        TUtils.WriteTitle("TestCascade...");
        Tests.RunCascade(State.Off,State.Off, State.Off);
        Tests.RunCascade(State.Off, State.Low, State.Low);
        Tests.RunCascade(State.Off, State.High, State.High);
        Tests.RunCascade(State.Off, State.Error, State.Off);
        Tests.RunCascade(State.High, State.Low, State.Low);
        Tests.RunCascade(State.Low, State.High, State.High);
    }
}

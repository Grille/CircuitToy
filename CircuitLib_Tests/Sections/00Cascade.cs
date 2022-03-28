namespace CircuitLib_Tests;

partial class Section
{
    public static void S00Cascade()
    {
        TUtils.WriteTitle("TestCascade...");
        Tests.RunCascade(State.Off,State.Off);
        Tests.RunCascade(State.Off, State.Low);
        Tests.RunCascade(State.Off, State.High);
        Tests.RunCascade(State.Off, State.Error);
        Tests.RunCascade(State.High, State.Low);
        Tests.RunCascade(State.Low, State.High);
    }
}

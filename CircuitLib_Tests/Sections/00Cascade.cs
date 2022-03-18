namespace CircuitLib_Tests;

partial class Section
{
    public static void S00Cascade()
    {
        TUtils.WriteTitle("TestCascade...");
        Tests.RunCascade(true);
        Tests.RunCascade(false);
    }
}

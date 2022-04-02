namespace CircuitLib_Tests;

partial class Programm
{
    static void Main(string[] args)
    {
        TUtils.CatchExeptions = false;

        Section.S00Cascade();
        Section.S01Primitives();
        Section.S02Network();
        Section.S03NetworkInteraction();
        Section.S04Circuits();
        Section.S05Serialization();

        TUtils.WriteResults();
    }


}


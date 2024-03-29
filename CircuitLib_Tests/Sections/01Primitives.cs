﻿namespace CircuitLib_Tests;

partial class Section
{
    public static void S01Primitives()
    {
        TUtils.WriteTitle("TestPrimitives...");
        Tests.RunIOTable<BufferGate>("0->0");
        Tests.RunIOTable<BufferGate>("1->1");
        Tests.RunIOTable<BufferGate>("Z->E");
        Tests.RunIOTable<BufferGate>("E->E");

        Tests.RunIOTable<AndGate>("00->0");
        Tests.RunIOTable<AndGate>("01->0");
        Tests.RunIOTable<AndGate>("10->0");
        Tests.RunIOTable<AndGate>("11->1");

        Tests.RunIOTable<OrGate>("00->0");
        Tests.RunIOTable<OrGate>("01->1");
        Tests.RunIOTable<OrGate>("10->1");
        Tests.RunIOTable<OrGate>("11->1");

        Tests.RunIOTable<XorGate>("00->0");
        Tests.RunIOTable<XorGate>("01->1");
        Tests.RunIOTable<XorGate>("10->1");
        Tests.RunIOTable<XorGate>("11->0");

        Tests.RunIOTable<NotGate>("0->1");
        Tests.RunIOTable<NotGate>("1->0");
        Tests.RunIOTable<NotGate>("Z->E");
        Tests.RunIOTable<NotGate>("E->E");

        Tests.RunIOTable<NAndGate>("00->1");
        Tests.RunIOTable<NAndGate>("01->1");
        Tests.RunIOTable<NAndGate>("10->1");
        Tests.RunIOTable<NAndGate>("11->0");

        Tests.RunIOTable<NOrGate>("00->1");
        Tests.RunIOTable<NOrGate>("01->0");
        Tests.RunIOTable<NOrGate>("10->0");
        Tests.RunIOTable<NOrGate>("11->0");

        Tests.RunIOTable<XNorGate>("00->1");
        Tests.RunIOTable<XNorGate>("01->0");
        Tests.RunIOTable<XNorGate>("10->0");
        Tests.RunIOTable<XNorGate>("11->1");

        Tests.RunIOTable<TriState>("00->Z");
        Tests.RunIOTable<TriState>("01->0");
        Tests.RunIOTable<TriState>("10->Z");
        Tests.RunIOTable<TriState>("11->1");

        Tests.RunIOTable<PullDown>("0->0");
        Tests.RunIOTable<PullDown>("1->1");
        Tests.RunIOTable<PullDown>("Z->0");
        Tests.RunIOTable<PullDown>("E->E");

        Tests.RunIOTable<PullUp>("0->0");
        Tests.RunIOTable<PullUp>("1->1");
        Tests.RunIOTable<PullUp>("Z->1");
        Tests.RunIOTable<PullUp>("E->E");
    }
}

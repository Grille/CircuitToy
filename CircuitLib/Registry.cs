using System;
using System.IO;

using CircuitLib.Primitives;

namespace CircuitLib;

public class Registry
{
    public string DefaultPath;
    public void RegisterType<T>(string path) where T : Node
    {
        RegisterType(typeof(T), path);
    }

    public void RegisterType(Type type, string path)
    {
        if (type == typeof(Circuit))
            throw new ArgumentException("Circuit no Type");

        string fullPath = Path.Combine(DefaultPath, path);
        Console.WriteLine($"{type.FullName} {fullPath}");
    }

    public void RegisterCircuit(Circuit node)
    {
    
    }

    public void InitStd()
    {
        DefaultPath = "std/IO";
        RegisterType<Input>("Input");
        RegisterType<Output>("Output");

        DefaultPath = "std/Primitives";
        RegisterType<BufferGate>("BufferGate");
        RegisterType<AndGate>("AndGate");
        RegisterType<OrGate>("OrGate");
        RegisterType<XorGate>("XorGate");

        RegisterType<NotGate>("NotGate");
        RegisterType<NAndGate>("NAndGate");
        RegisterType<NOrGate>("NOrGate");
        RegisterType<XNorGate>("XNorGate");

        RegisterType<TriState>("TriState");
        RegisterType<PullDown>("PullDown");
        RegisterType<PullUp>("PullUp");
    }
}

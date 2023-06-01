using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

using CircuitLib.Primitives;
using GGL.IO;
using GGL.IO.Compression;
using System.IO;

namespace CircuitLib.Serialization;

public enum SaveFileState
{
    OK,
    Error,
    UnknownFileType,
}
public class SaveFile
{
    const int MagicNumber = 1255423642;
    const int FileVersion = 0;

    public Circuit Circuit;
    public SaveFileState State;

    private SaveFile(SaveFileState state)
    {
        Circuit = new Circuit();
        State = state;
    }

    private SaveFile(SaveFileState state, Circuit circuit)
    {
        Circuit = circuit;
        State = state;
    }

    public static void Save(string path, Circuit circuit)
    {
        using var fs = new FileStream(path, FileMode.Create);
        Save(fs, circuit);
    }

    public static SaveFile Load(string path)
    {
        using var fs = new FileStream(path, FileMode.Open);
        return Load(fs);
    }

    public static void Save(Stream stream, Circuit circuit)
    {
        using var bs = new BinarySerializer(stream);
        bs.WriteHead();
        bs.WriteNode(circuit);
    }

    public static SaveFile Load(Stream stream)
    {
        using var bd = new BinaryDeserializer(stream);
        bd.ReadHead();
        var circuit = (Circuit)bd.ReadNode();

        return new SaveFile(SaveFileState.OK, circuit);
    }
}

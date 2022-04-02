using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

using CircuitLib.Primitives;
using GGL.IO;

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
        using var bw = new BinaryViewWriter(path);
        bw.WriteInt32(MagicNumber);
        bw.WriteInt32(FileVersion);
        bw.WriteString(".lcp", LengthPrefix.Byte, CharSizePrefix.Byte);

        bw.BeginDeflateSection();
        CircuitSerialization.WriteNode(bw, circuit);
        bw.EndDeflateSection();

        bw.Dispose();
    }

    public static SaveFile Load(string path)
    {
        using var br = new BinaryViewReader(path);

        int magicNumber = br.ReadInt32();
        if (magicNumber != MagicNumber)
            return new SaveFile(SaveFileState.UnknownFileType);

        int version = br.ReadInt32();

        string file = br.ReadString(LengthPrefix.Byte, CharSizePrefix.Byte);

        br.BeginDeflateSection();
        var circuit = (Circuit)CircuitDeserialization.ReadNode(br);
        br.EndDeflateSection();

        br.Dispose();

        return new SaveFile(SaveFileState.OK, circuit);
    }
}

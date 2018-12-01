using System;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal interface IBinaryReader
    {
        long Position { get; set; }

        Endianness Endianness { get; set; }

        byte ReadByte();
        bool ReadBool();

        short ReadInt16();
        int ReadInt32();
        long ReadInt64();

        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();

        float ReadFloat();
        double ReadDouble();
        Guid ReadGuid();

        uint ReadVarlenUInt32();
        ulong ReadVarlenUInt64();

        string ReadString(Encoding encoding);
        string ReadString(Encoding encoding, int length);

        byte[] ReadByteArray();
        byte[] ReadByteArray(int size);
    }
}
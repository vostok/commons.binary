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

        [NotNull] string ReadString(Encoding encoding);
        [NotNull] string ReadString(Encoding encoding, int size);

        [NotNull] byte[] ReadByteArray();
        [NotNull] byte[] ReadByteArray(int size);
    }
}
using System;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal interface IBinaryWriter
    {
        long Position { get; set; }

        Endianness Endianness { get; set; }

        void Write(byte value);
        void Write(bool value);

        void Write(short value);
        void Write(int value);
        void Write(long value);

        void Write(ushort value);
        void Write(uint value);
        void Write(ulong value);

        void Write(float value);
        void Write(double value);
        void Write(Guid value);

        void WriteVarlen(uint value);
        void WriteVarlen(ulong value);

        void WriteWithLength(string value, Encoding encoding);
        void WriteWithoutLength(string value, Encoding encoding);

        void WriteWithLength(byte[] value);
        void WriteWithLength(byte[] value, int offset, int length);
        void WriteWithoutLength(byte[] value);
        void WriteWithoutLength(byte[] value, int offset, int length);
    }
}

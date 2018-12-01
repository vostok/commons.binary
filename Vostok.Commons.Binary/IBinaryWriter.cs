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

        void WriteWithLength([NotNull] string value, [NotNull] Encoding encoding);
        void WriteWithoutLength([NotNull] string value, [NotNull] Encoding encoding);

        void WriteWithLength([NotNull] byte[] value);
        void WriteWithLength([NotNull] byte[] value, int offset, int length);
        void WriteWithoutLength([NotNull] byte[] value);
        void WriteWithoutLength([NotNull] byte[] value, int offset, int length);
    }
}

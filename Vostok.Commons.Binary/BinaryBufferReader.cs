using System;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal class BinaryBufferReader : IBinaryReader
    {
        public BinaryBufferReader([NotNull] byte[] buffer, long position)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Position = position;
        }

        private static readonly Endianness SystemEndianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

        public byte[] Buffer { get; }

        public long Position { get; set; }

        public long BytesRemaining => Buffer.Length - Position;

        public Endianness Endianness { get; set; } = SystemEndianness;

        public byte ReadByte() => throw new NotImplementedException();

        public bool ReadBool() => throw new NotImplementedException();

        public short ReadInt16() => throw new NotImplementedException();

        public int ReadInt32() => throw new NotImplementedException();

        public long ReadInt64() => throw new NotImplementedException();

        public ushort ReadUInt16() => throw new NotImplementedException();

        public uint ReadUInt32() => throw new NotImplementedException();

        public ulong ReadUInt64() => throw new NotImplementedException();

        public float ReadFloat() => throw new NotImplementedException();

        public double ReadDouble() => throw new NotImplementedException();

        public Guid ReadGuid() => throw new NotImplementedException();

        public uint ReadVarlenUInt32() => throw new NotImplementedException();

        public ulong ReadVarlenUInt64() => throw new NotImplementedException();

        public string ReadString(Encoding encoding) => throw new NotImplementedException();

        public string ReadString(Encoding encoding, int length) => throw new NotImplementedException();

        public byte[] ReadByteArray() => throw new NotImplementedException();

        public byte[] ReadByteArray(int size) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckBounds(int size)
        {
            if (size > BytesRemaining)
                throw new IndexOutOfRangeException($"Requested to read {size} bytes from buffer, but it only has {BytesRemaining} bytes remaining.");
        }
    }
}
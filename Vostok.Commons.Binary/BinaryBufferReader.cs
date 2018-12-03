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

        public byte ReadByte() => Buffer[Position++];

        public bool ReadBool() => Buffer[Position++] != 0;

        public unsafe short ReadInt16()
        {
            const int ValueSize = sizeof(short);

            CheckBounds(ValueSize);

            short result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(short*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe int ReadInt32()
        {
            const int ValueSize = sizeof(int);

            CheckBounds(ValueSize);

            int result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(int*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe long ReadInt64()
        {
            const int ValueSize = sizeof(long);

            CheckBounds(ValueSize);

            long result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(long*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe ushort ReadUInt16()
        {
            const int ValueSize = sizeof(ushort);

            CheckBounds(ValueSize);

            ushort result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(ushort*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe uint ReadUInt32()
        {
            const int ValueSize = sizeof(uint);

            CheckBounds(ValueSize);

            uint result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(uint*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe ulong ReadUInt64()
        {
            const int ValueSize = sizeof(ulong);

            CheckBounds(ValueSize);

            ulong result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(ulong*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe float ReadFloat()
        {
            const int ValueSize = sizeof(float);

            CheckBounds(ValueSize);

            float result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(float*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe double ReadDouble()
        {
            const int ValueSize = sizeof(double);

            CheckBounds(ValueSize);

            double result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(double*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe Guid ReadGuid()
        {
            const int ValueSize = 16;

            CheckBounds(ValueSize);

            Guid result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(Guid*) ptr;

            Position += ValueSize;

            return result;
        }

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
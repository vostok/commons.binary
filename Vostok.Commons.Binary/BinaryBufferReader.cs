using System;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal class BinaryBufferReader : IBinaryReader
    {
        public BinaryBufferReader([NotNull] byte[] buffer, int position)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Position = position;

            if (buffer.LongLength > int.MaxValue)
                throw new ArgumentException($"Buffer was too large (it's length is {buffer.LongLength}).");

            if (position > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(position), $"Starting position {position} was outside of buffer (it's length is {buffer.Length}).");
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

            EnsureSufficientSizeRemaining(ValueSize);

            short result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(short*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe int ReadInt32()
        {
            const int ValueSize = sizeof(int);

            EnsureSufficientSizeRemaining(ValueSize);

            int result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(int*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe long ReadInt64()
        {
            const int ValueSize = sizeof(long);

            EnsureSufficientSizeRemaining(ValueSize);

            long result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(long*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe ushort ReadUInt16()
        {
            const int ValueSize = sizeof(ushort);

            EnsureSufficientSizeRemaining(ValueSize);

            ushort result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(ushort*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe uint ReadUInt32()
        {
            const int ValueSize = sizeof(uint);

            EnsureSufficientSizeRemaining(ValueSize);

            uint result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(uint*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe ulong ReadUInt64()
        {
            const int ValueSize = sizeof(ulong);

            EnsureSufficientSizeRemaining(ValueSize);

            ulong result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(ulong*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe float ReadFloat()
        {
            const int ValueSize = sizeof(float);

            EnsureSufficientSizeRemaining(ValueSize);

            float result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(float*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe double ReadDouble()
        {
            const int ValueSize = sizeof(double);

            EnsureSufficientSizeRemaining(ValueSize);

            double result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(double*) ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        public unsafe Guid ReadGuid()
        {
            const int ValueSize = 16;

            EnsureSufficientSizeRemaining(ValueSize);

            Guid result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(Guid*) ptr;

            Position += ValueSize;

            return result;
        }

        public uint ReadVarlenUInt32()
        {
            throw new NotImplementedException();
        }

        public ulong ReadVarlenUInt64()
        {
            throw new NotImplementedException();
        }

        public string ReadString(Encoding encoding)
        {
            return ReadString(encoding, ReadInt32());
        }

        public string ReadString(Encoding encoding, int size)
        {
            EnsureSufficientSizeRemaining(size);

            var result = encoding.GetString(Buffer, (int) Position, size);

            Position += size;

            return result;
        }

        public byte[] ReadByteArray()
        {
            return ReadByteArray(ReadInt32());
        }

        public byte[] ReadByteArray(int size)
        {
            EnsureSufficientSizeRemaining(size);

            var result = new byte[size];

            System.Buffer.BlockCopy(Buffer, (int) Position, result, 0, size);

            Position += size;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureSufficientSizeRemaining(int size)
        {
            if (size > BytesRemaining)
                throw new IndexOutOfRangeException($"Requested to read {size} bytes from buffer, but it only has {BytesRemaining} bytes remaining.");
        }
    }
}
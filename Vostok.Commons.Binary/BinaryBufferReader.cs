using System;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    /// <summary>
    /// <para>An implementation of <see cref="IBinaryReader"/> based on an in-memory byte array.</para>
    /// <para>Supports changing <see cref="Position"/> and <see cref="Endianness"/> on the fly.</para>
    /// <para>Not thread-safe.</para>
    /// </summary>
    [PublicAPI]
    internal class BinaryBufferReader : IBinaryReader
    {
        public BinaryBufferReader([NotNull] byte[] buffer, long position)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Position = position;

            if (buffer.LongLength > int.MaxValue)
                throw new ArgumentException($"Buffer was too large (it's length is {buffer.LongLength}).");

            if (position > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(position), $"Starting position {position} was outside of buffer (it's length is {buffer.Length}).");
        }

        public byte[] Buffer { get; }

        public long Position { get; set; }

        public long BytesRemaining => Buffer.Length - Position;

        public Endianness Endianness { get; set; } = EndiannessConverter.SystemEndianness;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() => Buffer[Position++];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool() => Buffer[Position++] != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe short ReadInt16()
        {
            const int ValueSize = sizeof(short);

            EnsureSufficientSizeRemaining(ValueSize);

            short result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(short*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int ReadInt32()
        {
            const int ValueSize = sizeof(int);

            EnsureSufficientSizeRemaining(ValueSize);

            int result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(int*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long ReadInt64()
        {
            const int ValueSize = sizeof(long);

            EnsureSufficientSizeRemaining(ValueSize);

            long result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(long*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint ReadUInt32()
        {
            const int ValueSize = sizeof(uint);

            EnsureSufficientSizeRemaining(ValueSize);

            uint result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(uint*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ulong ReadUInt64()
        {
            const int ValueSize = sizeof(ulong);

            EnsureSufficientSizeRemaining(ValueSize);

            ulong result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(ulong*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float ReadFloat()
        {
            const int ValueSize = sizeof(float);

            EnsureSufficientSizeRemaining(ValueSize);

            float result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(float*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double ReadDouble()
        {
            const int ValueSize = sizeof(double);

            EnsureSufficientSizeRemaining(ValueSize);

            double result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(double*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Guid ReadGuid()
        {
            const int ValueSize = 16;

            EnsureSufficientSizeRemaining(ValueSize);

            Guid result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(Guid*)ptr;

            Position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadVarlenUInt32()
        {
            var result = 0U;
            var shift = 0;
            var size = 0;
            var hasMore = true;

            while (hasMore)
            {
                var currentByte = Buffer[Position + size];

                if (size == sizeof(uint))
                {
                    result |= (uint)currentByte << shift;
                    size++;
                    break;
                }

                hasMore = (currentByte & 0x80) != 0;
                result |= (uint)(currentByte & 0x7F) << shift;
                shift += 7;
                size++;
            }

            Position += size;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadVarlenUInt64()
        {
            var result = 0UL;
            var shift = 0;
            var size = 0;
            var hasMore = true;

            while (hasMore)
            {
                var currentByte = Buffer[Position + size];

                if (size == sizeof(ulong))
                {
                    result |= (ulong)currentByte << shift;
                    size++;
                    break;
                }

                hasMore = (currentByte & 0x80) != 0;
                result |= (ulong)(currentByte & 0x7F) << shift;
                shift += 7;
                size++;
            }

            Position += size;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string ReadString(Encoding encoding)
        {
            var size = ReadInt32();

            EnsureSufficientSizeRemaining(size);

            var result = encoding.GetString(Buffer, (int)Position, size);

            Position += size;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string ReadShortString(Encoding encoding)
        {
            var size = ReadByte();

            EnsureSufficientSizeRemaining(size);

            var result = encoding.GetString(Buffer, (int)Position, size);

            Position += size;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadByteArray()
        {
            return ReadByteArray(ReadInt32());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadByteArray(int size)
        {
            EnsureSufficientSizeRemaining(size);

            var result = new byte[size];

            System.Buffer.BlockCopy(Buffer, (int)Position, result, 0, size);

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
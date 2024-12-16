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
    internal class ArraySegmentReader : IBinaryReader
    {
        private int position;

        public ArraySegmentReader(ArraySegment<byte> segment)
        {
            if (segment.Array == null)
                throw new ArgumentNullException(nameof(segment), "ArraySegment<byte> contains null array");
            Segment = segment;
            position = 0;
        }

        public ArraySegment<byte> Segment { get; }

        public long Position
        {
            get => position;
            set
            {
                if (value > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ArraySegmentReader)} does not support positions greater than int.MaxValue.");
                position = (int)value;
            }
        }

        public long BytesRemaining => Segment.Count - position;

        public Endianness Endianness { get; set; } = EndiannessConverter.SystemEndianness;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            const int ValueSize = sizeof(byte);

            EnsureSufficientSizeRemaining(ValueSize);

            var result = Segment.Array![Segment.Offset + position];

            position += ValueSize;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool()
        {
            const int ValueSize = sizeof(byte);

            EnsureSufficientSizeRemaining(ValueSize);

            var result = Segment.Array![Segment.Offset + position] != 0;

            position += ValueSize;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe short ReadInt16()
        {
            const int ValueSize = sizeof(short);

            EnsureSufficientSizeRemaining(ValueSize);

            short result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(short*)ptr;

            position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int ReadInt32()
        {
            const int ValueSize = sizeof(int);

            EnsureSufficientSizeRemaining(ValueSize);

            int result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(int*)ptr;

            position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long ReadInt64()
        {
            const int ValueSize = sizeof(long);

            EnsureSufficientSizeRemaining(ValueSize);

            long result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(long*)ptr;

            position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort ReadUInt16()
        {
            const int ValueSize = sizeof(ushort);

            EnsureSufficientSizeRemaining(ValueSize);

            ushort result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(ushort*)ptr;

            position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint ReadUInt32()
        {
            const int ValueSize = sizeof(uint);

            EnsureSufficientSizeRemaining(ValueSize);

            uint result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(uint*)ptr;

            position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ulong ReadUInt64()
        {
            const int ValueSize = sizeof(ulong);

            EnsureSufficientSizeRemaining(ValueSize);

            ulong result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(ulong*)ptr;

            position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float ReadFloat()
        {
            const int ValueSize = sizeof(float);

            EnsureSufficientSizeRemaining(ValueSize);

            float result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(float*)ptr;

            position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double ReadDouble()
        {
            const int ValueSize = sizeof(double);

            EnsureSufficientSizeRemaining(ValueSize);

            double result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(double*)ptr;

            position += ValueSize;

            return EndiannessConverter.Convert(result, Endianness);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Guid ReadGuid()
        {
            const int ValueSize = 16;

            EnsureSufficientSizeRemaining(ValueSize);

            Guid result;

            fixed (byte* ptr = &Segment.Array![Segment.Offset + position])
                result = *(Guid*)ptr;

            position += ValueSize;

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
                EnsureSufficientSizeRemaining(size + 1);

                var currentByte = Segment.Array![Segment.Offset + position + size];

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

            position += size;

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
                EnsureSufficientSizeRemaining(size + 1);

                var currentByte = Segment.Array![Segment.Offset + position + size];

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

            position += size;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string ReadString(Encoding encoding)
        {
            var size = ReadInt32();

            EnsureSufficientSizeRemaining(size);

            var result = encoding.GetString(Segment.Array!, Segment.Offset + position, size);

            position += size;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string ReadShortString(Encoding encoding)
        {
            var size = ReadByte();

            EnsureSufficientSizeRemaining(size);

            var result = encoding.GetString(Segment.Array!, Segment.Offset + position, size);

            position += size;

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

            Buffer.BlockCopy(Segment.Array!, Segment.Offset + position, result, 0, size);

            position += size;

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
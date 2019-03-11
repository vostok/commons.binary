using System;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    /// <summary>
    /// <para>An implementation of <see cref="IBinaryWriter"/> based on an in-memory byte array.</para>
    /// <para>Can dynamically resize during writing to accomodate more data (uses 2x growth by default).</para>
    /// <para>Supports changing <see cref="Position"/> and <see cref="Endianness"/> on the fly.</para>
    /// <para>Not thread-safe.</para>
    /// </summary>
    [PublicAPI]
    internal class BinaryBufferWriter : IBinaryWriter
    {
        private int position;

        public BinaryBufferWriter(byte[] buffer)
        {
            Buffer = buffer;

            Reset();
        }

        public BinaryBufferWriter(int initialCapacity)
        {
            Reset(initialCapacity);
        }

        public Endianness Endianness { get; set; } = EndiannessConverter.SystemEndianness;

        public byte[] Buffer { get; private set; }

        public int Length { get; private set; }

        public long Position
        {
            get => position;
            set
            {
                if (value > int.MaxValue)
                    throw new OverflowException();

                if (value < 0 || value > Buffer.Length)
                    throw new IndexOutOfRangeException();

                position = (int)value;

                if (position > Length)
                    Length = position;
            }
        }

        public ArraySegment<byte> FilledSegment => new ArraySegment<byte>(Buffer, 0, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value)
        {
            EnsureCapacity(1);

            Buffer[position++] = value;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(bool value)
        {
            Write(value ? (byte)1 : (byte)0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(short value)
        {
            const int ValueSize = sizeof(short);

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(short*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(int value)
        {
            const int ValueSize = sizeof(int);

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(int*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(long value)
        {
            const int ValueSize = sizeof(long);

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(long*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(ushort value)
        {
            const int ValueSize = sizeof(ushort);

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(ushort*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(uint value)
        {
            const int ValueSize = sizeof(uint);

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(uint*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(ulong value)
        {
            const int ValueSize = sizeof(ulong);

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(ulong*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(float value)
        {
            const int ValueSize = sizeof(float);

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(float*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(double value)
        {
            const int ValueSize = sizeof(double);

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(double*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(Guid value)
        {
            const int ValueSize = 16;

            value = EndiannessConverter.Convert(value, Endianness);

            EnsureCapacity(ValueSize);

            fixed (byte* ptr = &Buffer[position])
                *(Guid*)ptr = value;

            position += ValueSize;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteVarlen(uint value)
        {
            EnsureCapacity(sizeof(uint) + 1);

            var bytesWritten = 0;

            do
            {
                if (bytesWritten == sizeof(uint))
                {
                    Buffer[position + bytesWritten] = (byte)value;
                    bytesWritten++;
                    break;
                }

                var currentByte = (byte)(value & 0x7F);
                value >>= 7;
                if (value > 0)
                    currentByte |= 0x80;
                Buffer[position + bytesWritten] = currentByte;
                bytesWritten++;
            } while (value > 0);

            position += bytesWritten;

            IncreaseLengthIfNeeded();

            return bytesWritten;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteVarlen(ulong value)
        {
            EnsureCapacity(sizeof(ulong) + 1);

            var bytesWritten = 0;

            do
            {
                if (bytesWritten == sizeof(ulong))
                {
                    Buffer[position + bytesWritten] = (byte)value;
                    bytesWritten++;
                    break;
                }

                var currentByte = (byte)(value & 0x7F);
                value >>= 7;
                if (value > 0)
                    currentByte |= 0x80;
                Buffer[position + bytesWritten] = currentByte;
                bytesWritten++;
            } while (value > 0);

            position += bytesWritten;

            IncreaseLengthIfNeeded();

            return bytesWritten;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithLength(string value, Encoding encoding)
        {
            EnsureCapacity(encoding.GetMaxByteCount(value.Length) + sizeof(int));

            var byteCount = encoding.GetBytes(value, 0, value.Length, Buffer, position + sizeof(int));

            Write(byteCount);

            position += byteCount;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithoutLength(string value, Encoding encoding)
        {
            EnsureCapacity(encoding.GetMaxByteCount(value.Length));

            position += encoding.GetBytes(value, 0, value.Length, Buffer, position);

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithLength(byte[] value)
        {
            WriteWithLength(value, 0, value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithLength(byte[] value, int offset, int length)
        {
            EnsureCapacity(length + sizeof(int));

            Write(length);

            System.Buffer.BlockCopy(value, offset, Buffer, position, length);

            position += length;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithoutLength(byte[] value)
        {
            WriteWithoutLength(value, 0, value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithoutLength(byte[] value, int offset, int length)
        {
            EnsureCapacity(length);

            System.Buffer.BlockCopy(value, offset, Buffer, position, length);

            position += length;

            IncreaseLengthIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset(int neededCapacity = 0)
        {
            if (Buffer == null || Buffer.Length < neededCapacity)
                Buffer = new byte[neededCapacity];

            position = 0;
            Length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int neededBytes)
        {
            var remainingBytes = Buffer.Length - position;
            if (remainingBytes >= neededBytes)
                return;

            var newCapacity = Buffer.Length + Math.Max(neededBytes - remainingBytes, Buffer.Length);
            var newBuffer = new byte[newCapacity];

            System.Buffer.BlockCopy(Buffer, 0, newBuffer, 0, Length);

            Buffer = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncreaseLengthIfNeeded()
        {
            if (position > Length)
                Length = position;
        }
    }
}

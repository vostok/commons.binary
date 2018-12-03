using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal class BinaryStreamWriter : IBinaryWriter
    {
        private const int BufferSize = 16;

        private readonly Stream stream;
        private readonly BinaryBufferWriter buffer;

        public BinaryStreamWriter([NotNull] Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new AggregateException($"Output stream of type '{stream.GetType().Name}' was not writable.");

            buffer = new BinaryBufferWriter(BufferSize);
        }

        public long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        public Endianness Endianness
        {
            get => buffer.Endianness;
            set => buffer.Endianness = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value)
            => stream.WriteByte(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(bool value)
            => Write(value ? (byte) 1 : (byte) 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(float value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(double value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Guid value)
        {
            buffer.Reset();
            buffer.Write(value);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteVarlen(uint value)
        {
            buffer.Reset();

            var size = buffer.WriteVarlen(value);

            Flush();

            return size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteVarlen(ulong value)
        {
            buffer.Reset();

            var size = buffer.WriteVarlen(value);

            Flush();

            return size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithLength(string value, Encoding encoding)
        {
            WriteWithLength(encoding.GetBytes(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithoutLength(string value, Encoding encoding)
        {
            WriteWithoutLength(encoding.GetBytes(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithLength(byte[] value)
        {
            Write(value.Length);
            WriteWithoutLength(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithLength(byte[] value, int offset, int length)
        {
            Write(length);
            WriteWithoutLength(value, offset, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithoutLength(byte[] value)
        {
            stream.Write(value, 0, value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWithoutLength(byte[] value, int offset, int length)
        {
            stream.Write(value, offset, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Flush()
        {
            stream.Write(buffer.Buffer, 0, buffer.Length);
        }
    }
}

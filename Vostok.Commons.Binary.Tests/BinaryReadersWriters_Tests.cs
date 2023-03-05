using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace Vostok.Commons.Binary.Tests
{
    [TestFixture]
    internal class BinaryReadersWriters_Tests
    {
        [TestCase(byte.MinValue)]
        [TestCase(byte.MaxValue)]
        [TestCase((byte)0xF0)]
        public void Should_correctly_write_and_read_byte_values(byte value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadByte());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_correctly_write_and_read_bool_values(bool value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadBool());
        }

        [TestCase(short.MinValue)]
        [TestCase(short.MinValue + 1)]
        [TestCase(short.MaxValue)]
        [TestCase(short.MaxValue - 1)]
        [TestCase((short)0)]
        [TestCase((short)1)]
        [TestCase((short)-1)]
        public void Should_correctly_write_and_read_int16_values(short value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadInt16());
        }

        [TestCase(ushort.MinValue)]
        [TestCase(ushort.MaxValue)]
        [TestCase((ushort)(ushort.MinValue + 1))]
        [TestCase((ushort)(ushort.MaxValue - 1))]
        [TestCase((ushort)0)]
        [TestCase((ushort)1)]
        public void Should_correctly_write_and_read_uint16_values(ushort value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadUInt16());
        }

        [TestCase(int.MinValue)]
        [TestCase(int.MinValue + 1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MaxValue - 1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public void Should_correctly_write_and_read_int32_values(int value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadInt32());
        }

        [TestCase(uint.MinValue)]
        [TestCase(uint.MinValue + 1)]
        [TestCase(uint.MaxValue)]
        [TestCase(uint.MaxValue - 1)]
        [TestCase(0U)]
        [TestCase(1U)]
        public void Should_correctly_write_and_read_uint32_values(uint value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadUInt32());
        }

        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(0L)]
        [TestCase(1L)]
        [TestCase(-1L)]
        public void Should_correctly_write_and_read_int64_values(long value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadInt64());
        }

        [TestCase(ulong.MinValue)]
        [TestCase(ulong.MinValue + 1)]
        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MaxValue - 1)]
        [TestCase(0UL)]
        [TestCase(1UL)]
        public void Should_correctly_write_and_read_uint64_values(ulong value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadUInt64());
        }

        [TestCase(uint.MinValue)]
        [TestCase(uint.MinValue + 1)]
        [TestCase(uint.MaxValue)]
        [TestCase(uint.MaxValue - 1)]
        [TestCase(0U)]
        [TestCase(1U)]
        [TestCase(1U << 1)]
        [TestCase(1U << 2)]
        [TestCase(1U << 4)]
        [TestCase(1U << 5)]
        [TestCase(1U << 7)]
        [TestCase(1U << 8)]
        [TestCase(1U << 10)]
        [TestCase((1U << 11) + 151)]
        [TestCase(1U << 12)]
        [TestCase(1U << 16)]
        [TestCase((1U << 17) - 54333)]
        [TestCase(1U << 21)]
        [TestCase(1U << 31)]
        public void Should_correctly_write_and_read_varlen_uint32_values(uint value)
        {
            Test(value, (item, writer) => writer.WriteVarlen(item), reader => reader.ReadVarlenUInt32());
        }

        [TestCase(ulong.MinValue)]
        [TestCase(ulong.MinValue + 1)]
        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MaxValue - 1)]
        [TestCase(0UL)]
        [TestCase(1UL)]
        [TestCase(1UL << 1)]
        [TestCase(1UL << 2)]
        [TestCase(1UL << 4)]
        [TestCase(1UL << 5)]
        [TestCase(1UL << 7)]
        [TestCase(1UL << 8)]
        [TestCase(1UL << 10)]
        [TestCase((1UL << 11) + 151)]
        [TestCase(1UL << 12)]
        [TestCase(1UL << 16)]
        [TestCase((1UL << 17) - 54333)]
        [TestCase(1UL << 21)]
        [TestCase(1UL << 31)]
        [TestCase((1UL << 39) - 4334234324)]
        [TestCase((1UL << 49) + 432535)]
        [TestCase(1UL << 55)]
        [TestCase(1UL << 63)]
        public void Should_correctly_write_and_read_varlen_uint64_values(ulong value)
        {
            Test(value, (item, writer) => writer.WriteVarlen(item), reader => reader.ReadVarlenUInt64());
        }

        [TestCase(0f)]
        [TestCase(float.MinValue)]
        [TestCase(float.MaxValue)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        [TestCase(float.NaN)]
        [TestCase(float.Epsilon)]
        [TestCase(-1.1111111111f)]
        [TestCase(0.43353543f)]
        [TestCase(1 / 3f)]
        [TestCase(-1 / 10f)]
        public void Should_correctly_write_and_read_float_values(float value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadFloat());
        }

        [TestCase(0d)]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.NaN)]
        [TestCase(double.Epsilon)]
        [TestCase(-1.1111111111d)]
        [TestCase(0.43353543d)]
        [TestCase(1 / 3d)]
        [TestCase(-1 / 10d)]
        public void Should_correctly_write_and_read_double_values(double value)
        {
            Test(value, (item, writer) => writer.Write(item), reader => reader.ReadDouble());
        }

        [Test]
        public void Should_correctly_write_and_read_guid_values()
        {
            Test(Guid.Empty, (item, writer) => writer.Write(item), reader => reader.ReadGuid());
            Test(Guid.NewGuid(), (item, writer) => writer.Write(item), reader => reader.ReadGuid());
        }

        [TestCase("abcdxyz")]
        [TestCase("ABCDXYZ")]
        [TestCase("0123456789")]
        [TestCase("кириллица")]
        [TestCase("àáâãäå¸æçýþÿ")]
        [TestCase("//\\        $#'`~\"")]
        [TestCase("-_?:;&()*^:%@!<>[]{}=+-")]
        public void Should_correctly_write_and_read_string_values_in_utf8_encoding(string value)
        {
            Test(value, (item, writer) => writer.WriteWithLength(item), reader => reader.ReadString());
        }

        [Test]
        public void Should_correctly_write_and_read_string_values_in_various_encodings()
        {
            var bytes = new byte[4096];
            var rng = new Random();

            rng.NextBytes(bytes);
            Test(Encoding.UTF8.GetString(bytes), (value, writer) => writer.WriteWithLength(value), reader => reader.ReadString());

            rng.NextBytes(bytes);
            Test(Encoding.UTF32.GetString(bytes), (value, writer) => writer.WriteWithLength(value), reader => reader.ReadString());

            rng.NextBytes(bytes);
            Test(Encoding.Unicode.GetString(bytes), (value, writer) => writer.WriteWithLength(value), reader => reader.ReadString());
        }

        [TestCase((byte)0xFF)]
        [TestCase((byte)0xFF, (byte)0xAB)]
        [TestCase((byte)0xC0, (byte)0xFF, (byte)0xEE, (byte)0xBA, (byte)0xBE)]
        public void Should_correctly_write_and_read_byte_array_values(params byte[] value)
        {
            Test(value, (item, writer) => writer.WriteWithLength(item), reader => reader.ReadByteArray());
            Test(value, (item, writer) => writer.WriteWithoutLength(item), reader => reader.ReadByteArray(value.Length));
        }


#if NET6_0_OR_GREATER
        [TestCase((byte)0xFF)]
        [TestCase((byte)0xFF, (byte)0xAB)]
        [TestCase((byte)0xC0, (byte)0xFF, (byte)0xEE, (byte)0xBA, (byte)0xBE)]
        public void Should_correctly_write_and_read_bytes_span_values(params byte[] value)
        {
            Test(value.AsSpan(), (item, writer) => writer.WriteWithLength(item), reader => reader.ReadBytesSpan());
            Test(value.AsSpan(), (item, writer) => writer.WriteWithoutLength(item), reader => reader.ReadBytesSpan(value.Length));
        }
#endif

        [Test]
        public void Should_correctly_write_and_read_timespan_values()
        {
            Test(TimeSpan.Zero, (item, writer) => writer.Write(item), reader => reader.ReadTimeSpan());
            Test(TimeSpan.MinValue, (item, writer) => writer.Write(item), reader => reader.ReadTimeSpan());
            Test(TimeSpan.MaxValue, (item, writer) => writer.Write(item), reader => reader.ReadTimeSpan());
            Test(1.Minutes(), (item, writer) => writer.Write(item), reader => reader.ReadTimeSpan());
            Test(5.Hours(), (item, writer) => writer.Write(item), reader => reader.ReadTimeSpan());
            Test(54532.Ticks(), (item, writer) => writer.Write(item), reader => reader.ReadTimeSpan());
        }

        [Test]
        public void Should_correctly_write_and_read_datetime_values()
        {
            Test(DateTime.Now, (item, writer) => writer.Write(item), reader => reader.ReadDateTime());
            Test(DateTime.UtcNow, (item, writer) => writer.Write(item), reader => reader.ReadDateTime());
        }

        [TestCase]
        [TestCase(1)]
        [TestCase(1, 2, 3)]
        public void Should_correctly_write_and_read_arrays(params int[] values)
        {
            Test(values, (item, writer) => writer.WriteCollection(item, (w, e) => w.Write(e)), reader => reader.ReadArray(r => r.ReadInt32()));
        }

        [TestCase]
        [TestCase(1)]
        [TestCase(1, 2, 3)]
        public void Should_correctly_write_and_read_lists(params int[] values)
        {
            var list = values.ToList();

            Test(list, (item, writer) => writer.WriteCollection(item, (w, e) => w.Write(e)), reader => reader.ReadList(r => r.ReadInt32()));
        }

        [TestCase]
        [TestCase(1)]
        [TestCase(1, 2, 3)]
        public void Should_correctly_write_and_read_sets(params int[] values)
        {
            var set = new HashSet<int>(values);

            Test(set, (item, writer) => writer.WriteCollection(item, (w, e) => w.Write(e)), reader => reader.ReadSet(r => r.ReadInt32()));
        }

        [TestCase]
        [TestCase(1)]
        [TestCase(1, 2, 3)]
        public void Should_correctly_write_and_read_dictionaries(params int[] values)
        {
            var dictionary = values.ToDictionary(v => v, v => v + 1);

            Test(
                dictionary,
                (item, writer) => writer.WriteDictionary(item, (w, e) => w.Write(e), (w, e) => w.Write(e)),
                reader => reader.ReadDictionary(r => r.ReadInt32(), r => r.ReadInt32()));
        }

        [Test]
        public void Should_correctly_write_and_read_nullable_structs()
        {
            Test(
                null,
                (item, writer) => writer.WriteNullable(item, (w, i) => w.Write(i)),
                reader => reader.ReadNullableStruct(r => r.ReadInt32()));

            Test(
                456,
                (item, writer) => writer.WriteNullable(item, (w, i) => w.Write(i)),
                reader => reader.ReadNullableStruct(r => r.ReadInt32()));
        }

        [Test]
        public void Should_correctly_write_and_read_nullable_classes()
        {
            Test(
                null,
                (item, writer) => writer.WriteNullable(item, (w, i) => w.WriteWithLength(i)),
                reader => reader.ReadNullable(r => r.ReadString()));

            Test(
                Guid.NewGuid().ToString(),
                (item, writer) => writer.WriteNullable(item, (w, i) => w.WriteWithLength(i)),
                reader => reader.ReadNullable(r => r.ReadString()));
        }

#region Helpers 

        private static void Test<T>(T item, Action<T, IBinaryWriter> write, Func<IBinaryReader, T> read)
        {
            TestBufferWithBuffer(item, write, read, Endianness.Little, true);
            TestBufferWithBuffer(item, write, read, Endianness.Little, false);
            TestBufferWithBuffer(item, write, read, Endianness.Big, true);
            TestBufferWithBuffer(item, write, read, Endianness.Big, false);

            TestBufferWithStream(item, write, read, Endianness.Little, true);
            TestBufferWithStream(item, write, read, Endianness.Little, false);
            TestBufferWithStream(item, write, read, Endianness.Big, true);
            TestBufferWithStream(item, write, read, Endianness.Big, false);

            TestStreamWithStream(item, write, read, Endianness.Little, true);
            TestStreamWithStream(item, write, read, Endianness.Little, false);
            TestStreamWithStream(item, write, read, Endianness.Big, true);
            TestStreamWithStream(item, write, read, Endianness.Big, false);

            TestStreamWithBuffer(item, write, read, Endianness.Little, true);
            TestStreamWithBuffer(item, write, read, Endianness.Little, false);
            TestStreamWithBuffer(item, write, read, Endianness.Big, true);
            TestStreamWithBuffer(item, write, read, Endianness.Big, false);
        }

        private static void TestBufferWithBuffer<T>(
            T item,
            Action<T, IBinaryWriter> write,
            Func<IBinaryReader, T> read,
            Endianness endianness,
            bool useGarbage)
        {
            TestBufferWith(item, write, read, endianness, useGarbage, CreateBufferReader);
        }

        private static void TestBufferWithStream<T>(
            T item,
            Action<T, IBinaryWriter> write,
            Func<IBinaryReader, T> read,
            Endianness endianness,
            bool useGarbage)
        {
            TestBufferWith(item, write, read, endianness, useGarbage, CreateStreamReader);
        }

        private static void TestStreamWithStream<T>(
            T item,
            Action<T, IBinaryWriter> write,
            Func<IBinaryReader, T> read,
            Endianness endianness,
            bool useGarbage)
        {
            TestStreamWith(item, write, read, endianness, useGarbage, CreateStreamReader);
        }

        private static void TestStreamWithBuffer<T>(
            T item,
            Action<T, IBinaryWriter> write,
            Func<IBinaryReader, T> read,
            Endianness endianness,
            bool useGarbage)
        {
            TestStreamWith(item, write, read, endianness, useGarbage, CreateBufferReader);
        }

        private static void TestBufferWith<T>(
            T item,
            Action<T, IBinaryWriter> write,
            Func<IBinaryReader, T> read,
            Endianness endianness,
            bool useGarbage,
            Func<byte[], int, Endianness, IBinaryReader> createReader)
        {
            var writer = new BinaryBufferWriter(1)
            {
                Endianness = endianness
            };

            if (useGarbage)
                writer.Write(Guid.NewGuid());

            var lengthBefore = writer.Length;

            write(item, writer);

            var lengthAfter = writer.Length;

            if (useGarbage)
                writer.Write(Guid.NewGuid());

            var itemLength = lengthAfter - lengthBefore;

            itemLength.Should().BeGreaterThan(0);

            writer.Position.Should().Be(writer.Length);

            var reader = createReader(writer.Buffer, lengthBefore, endianness);

            var restoredItem = read(reader);

            Compare(item, restoredItem);

            reader.Position.Should().Be(lengthAfter);
        }

        private static void TestStreamWith<T>(
            T item,
            Action<T, IBinaryWriter> write,
            Func<IBinaryReader, T> read,
            Endianness endianness,
            bool useGarbage,
            Func<byte[], int, Endianness, IBinaryReader> createReader)
        {
            var stream = new MemoryStream(1);

            var writer = new BinaryStreamWriter(stream)
            {
                Endianness = endianness
            };

            if (useGarbage)
                writer.Write(Guid.NewGuid());

            var positionBefore = writer.Position;

            write(item, writer);

            var positionAfter = writer.Position;

            if (useGarbage)
                writer.Write(Guid.NewGuid());

            var itemLength = positionAfter - positionBefore;

            itemLength.Should().BeGreaterThan(0);

            writer.Position.Should().Be(stream.Position);

            var reader = createReader(stream.ToArray(), (int)positionBefore, endianness);

            var restoredItem = read(reader);

            Compare(item, restoredItem);

            reader.Position.Should().Be(positionAfter);
        }

        private static void Compare<T>(T original, T restored)
        {
            if (original is IEnumerable originalEnumerable && restored is IEnumerable restoredEnumerable)
            {
                restoredEnumerable.Should().Equal(originalEnumerable);
            }
            else
            {
                restored.Should().Be(original);
            }
        }

        private static BinaryBufferReader CreateBufferReader(byte[] buffer, int offset, Endianness endianness)
        {
            return new BinaryBufferReader(buffer, offset) {Endianness = endianness};
        }

        private static BinaryStreamReader CreateStreamReader(byte[] buffer, int offset, Endianness endianness)
        {
            var memoryStream = new MemoryStream(buffer) {Position = offset};

            var slowStream = new SlowReadStream(memoryStream);

            return new BinaryStreamReader(slowStream) {Endianness = endianness};
        }

        private class SlowReadStream : Stream
        {
            private readonly Stream stream;

            public SlowReadStream(Stream stream)
            {
                this.stream = stream;
            }

            public override bool CanRead => stream.CanRead;

            public override bool CanSeek => stream.CanSeek;

            public override bool CanWrite => false;

            public override long Length => stream.Length;

            public override long Position
            {
                get => stream.Position;
                set => stream.Position = value;
            }

            public override int Read(byte[] buffer, int offset, int count)
                => stream.Read(buffer, offset, Math.Min(count, 1));

            public override long Seek(long offset, SeekOrigin origin)
                => stream.Seek(offset, origin);

            public override void SetLength(long value)
                => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count)
                => throw new NotSupportedException();

            public override void Flush()
                => throw new NotSupportedException();
        }

#endregion
    }
}
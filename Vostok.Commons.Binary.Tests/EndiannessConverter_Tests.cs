using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Binary.Tests
{
    [TestFixture]
    internal class EndiannessConverter_Tests
    {
        private static readonly Endianness SystemEndianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
        private static readonly Endianness ForeignEndianness = BitConverter.IsLittleEndian ? Endianness.Big : Endianness.Little;

        [TestCase(short.MinValue)]
        [TestCase(short.MaxValue)]
        [TestCase((short) 0)]
        public void Should_correctly_swap_endianness_for_short_values(short value)
        {
            var converted = EndiannessConverter.Swap(value);

            var convertedBack = EndiannessConverter.Swap(converted);

            var convertedWithBitConverter = BitConverter.ToInt16(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

            converted.Should().Be(convertedWithBitConverter);

            convertedBack.Should().Be(value);
        }

        [TestCase(ushort.MinValue)]
        [TestCase(ushort.MaxValue)]
        public void Should_correctly_swap_endianness_for_ushort_values(ushort value)
        {
            var converted = EndiannessConverter.Swap(value);

            var convertedBack = EndiannessConverter.Swap(converted);

            var convertedWithBitConverter = BitConverter.ToUInt16(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

            converted.Should().Be(convertedWithBitConverter);

            convertedBack.Should().Be(value);
        }

        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        [TestCase(0)]
        public void Should_correctly_swap_endianness_for_int_values(int value)
        {
            var converted = EndiannessConverter.Swap(value);

            var convertedBack = EndiannessConverter.Swap(converted);

            var convertedWithBitConverter = BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

            converted.Should().Be(convertedWithBitConverter);

            convertedBack.Should().Be(value);
        }

        [TestCase(uint.MinValue)]
        [TestCase(uint.MaxValue)]
        public void Should_correctly_swap_endianness_for_uint_values(uint value)
        {
            var converted = EndiannessConverter.Swap(value);

            var convertedBack = EndiannessConverter.Swap(converted);

            var convertedWithBitConverter = BitConverter.ToUInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

            converted.Should().Be(convertedWithBitConverter);

            convertedBack.Should().Be(value);
        }

        [TestCase(long.MinValue)]
        [TestCase(long.MaxValue)]
        [TestCase(0L)]
        public void Should_correctly_swap_endianness_for_long_values(long value)
        {
            var converted = EndiannessConverter.Swap(value);

            var convertedBack = EndiannessConverter.Swap(converted);

            var convertedWithBitConverter = BitConverter.ToInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

            converted.Should().Be(convertedWithBitConverter);

            convertedBack.Should().Be(value);
        }

        [TestCase(ulong.MinValue)]
        [TestCase(ulong.MaxValue)]
        public void Should_correctly_swap_endianness_for_ulong_values(ulong value)
        {
            var converted = EndiannessConverter.Swap(value);

            var convertedBack = EndiannessConverter.Swap(converted);

            var convertedWithBitConverter = BitConverter.ToUInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

            converted.Should().Be(convertedWithBitConverter);

            convertedBack.Should().Be(value);
        }

        [TestCase(float.MinValue)]
        [TestCase(float.MaxValue)]
        [TestCase(0f)]
        [TestCase(0.001f)]
        [TestCase(-0.001f)]
        [TestCase(-653643.43423f)]
        [TestCase(float.Epsilon)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void Should_correctly_swap_endianness_for_float_values(float value)
        {
            var converted = EndiannessConverter.Swap(value);

            var convertedBack = EndiannessConverter.Swap(converted);

            var convertedWithBitConverter = BitConverter.ToSingle(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

            converted.Should().Be(convertedWithBitConverter);

            convertedBack.Should().Be(value);
        }

        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        [TestCase(0d)]
        [TestCase(0.001d)]
        [TestCase(-0.001d)]
        [TestCase(-653643.43423d)]
        [TestCase(double.Epsilon)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Should_correctly_swap_endianness_for_double_values(double value)
        {
            var converted = EndiannessConverter.Swap(value);

            var convertedBack = EndiannessConverter.Swap(converted);

            var convertedWithBitConverter = BitConverter.ToDouble(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

            converted.Should().Be(convertedWithBitConverter);

            convertedBack.Should().Be(value);
        }

        [TestCase("01020304-0506-0708-1020-304050607080", "04030201-0605-0807-1020-304050607080")]
        public void Should_correctly_swap_endianness_for_Guid_values(string guidString, string convertedGuidString)
        {
            var value = Guid.Parse(guidString);

            var expectedConvertedValue = Guid.Parse(convertedGuidString);
            
            var converted = EndiannessConverter.Swap(value);

            converted.Should().Be(expectedConvertedValue);
        }

        [TestCase(Endianness.Big)]
        [TestCase(Endianness.Little)]
        public unsafe void Should_correctly_convert_Guid_values(Endianness endianness)
        {
            var value = Guid.Parse("01020304-0506-0708-1020-304050607080");

            var expectedBytes = endianness == Endianness.Big
                ? new byte[]
                {
                    0x01, 0x02, 0x03, 0x04,
                    0x05, 0x06,
                    0x07, 0x08,
                    0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80
                }
                : new byte[]
                {
                    0x04, 0x03, 0x02, 0x01,
                    0x06, 0x05,
                    0x08, 0x07,
                    0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80
                };

            var converted = EndiannessConverter.Convert(value, endianness);

            fixed (byte* b = expectedBytes)
                converted.Should().Be(*(Guid*) b);
        }

        [Test]
        public void Convert_should_not_swap_byte_order_if_number_is_already_in_requested_endianness()
        {
            EndiannessConverter.Convert(short.MaxValue, SystemEndianness).Should().Be(short.MaxValue);
            EndiannessConverter.Convert(ushort.MaxValue, SystemEndianness).Should().Be(ushort.MaxValue);
            EndiannessConverter.Convert(int.MaxValue, SystemEndianness).Should().Be(int.MaxValue);
            EndiannessConverter.Convert(uint.MaxValue, SystemEndianness).Should().Be(uint.MaxValue);
            EndiannessConverter.Convert(long.MaxValue, SystemEndianness).Should().Be(long.MaxValue);
            EndiannessConverter.Convert(ulong.MaxValue, SystemEndianness).Should().Be(ulong.MaxValue);
            EndiannessConverter.Convert(float.MaxValue, SystemEndianness).Should().Be(float.MaxValue);
            EndiannessConverter.Convert(double.MaxValue, SystemEndianness).Should().Be(double.MaxValue);
        }

        [Test]
        public void Convert_should_swap_byte_order_if_number_is_not_in_requested_endianness()
        {
            EndiannessConverter.Convert(short.MaxValue, ForeignEndianness).Should().Be(EndiannessConverter.Swap(short.MaxValue));
            EndiannessConverter.Convert(ushort.MaxValue, ForeignEndianness).Should().Be(EndiannessConverter.Swap(ushort.MaxValue));
            EndiannessConverter.Convert(int.MaxValue, ForeignEndianness).Should().Be(EndiannessConverter.Swap(int.MaxValue));
            EndiannessConverter.Convert(uint.MaxValue, ForeignEndianness).Should().Be(EndiannessConverter.Swap(uint.MaxValue));
            EndiannessConverter.Convert(long.MaxValue, ForeignEndianness).Should().Be(EndiannessConverter.Swap(long.MaxValue));
            EndiannessConverter.Convert(ulong.MaxValue, ForeignEndianness).Should().Be(EndiannessConverter.Swap(ulong.MaxValue));
            EndiannessConverter.Convert(float.MaxValue, ForeignEndianness).Should().Be(EndiannessConverter.Swap(float.MaxValue));
            EndiannessConverter.Convert(double.MaxValue, ForeignEndianness).Should().Be(EndiannessConverter.Swap(double.MaxValue));
        }
    }
}

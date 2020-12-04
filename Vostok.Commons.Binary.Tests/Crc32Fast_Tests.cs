using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Binary.Tests
{
    [TestFixture]
    internal class Crc32Fast_Tests
    {
        private Random random;
        private byte[] data;

        [SetUp]
        public void TestSetup()
        {
            data = new byte[999];
            random = new Random();
            random.NextBytes(data);
        }

        [Test]
        public void Compute_should_return_zero_for_empty_data()
        {
            var hash = Crc32Fast.Compute(data, 0, 0);

            hash.Should().Be(0);
        }

        [Test]
        public void Compute_should_compute_incremental_hash_correctly()
        {
            var usualHash = Crc32Fast.Compute(data);
            var incrementalHash = PartitionData().Aggregate(0U, (current, segment) => Crc32Fast.Compute(segment, current));

            incrementalHash.Should().Be(usualHash);
        }

        [Test]
        public void Combine_should_correctly_combine_multiple_hashes()
        {
            var usualHash = Crc32Fast.Compute(data);
            var combinedHash = PartitionData().Aggregate(0U, (current, segment) => Crc32Fast.Combine(current, Crc32Fast.Compute(segment), segment.Count));

            combinedHash.Should().Be(usualHash);
        }

        [Test]
        public void Combine_should_return_first_hash_if_second_length_is_zero()
        {
            const uint crc1 = 100500;
            const uint crc2 = 0;
            const int length2 = 0;

            var combinedHash = Crc32Fast.Combine(crc1, crc2, length2);

            combinedHash.Should().Be(crc1);
        }

        [Test]
        public void Combine_should_return_second_hash_if_first_hash_is_zero()
        {
            const uint crc1 = 0;
            const uint crc2 = 100500;
            const int length2 = 123;

            var combinedHash = Crc32Fast.Combine(crc1, crc2, length2);

            combinedHash.Should().Be(crc2);
        }

        [Test]
        public void ComputeForZeros_should_return_same_result_as_compute_for_zero_filled_array()
        {
            const int minPrecomputedSize = 2 * 1024;
            const int maxPrecomputedSize = 32 * 1024 * 1024;

            var size = maxPrecomputedSize;

            for (var i = maxPrecomputedSize / 2; i >= minPrecomputedSize; i /= 2)
            {
                size += i;
            }

            size += random.Next(minPrecomputedSize);

            var referenceHash = Crc32Fast.Compute(new byte[size]);
            var actualHash = Crc32Fast.ComputeForZeros(size);

            actualHash.Should().Be(referenceHash);
        }

        private IEnumerable<ArraySegment<byte>> PartitionData()
        {
            var parts = random.Next(2, 10);
            var partSize = data.Length / parts;

            for (var offset = 0; offset < data.Length; offset += partSize)
                yield return new ArraySegment<byte>(data, offset, Math.Min(partSize, data.Length - offset));
        }
    }
}
using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Binary.Tests
{
    [TestFixture]
    public class ArraySegmentReader_Tests
    {
        [Test]
        public void Test_bound_check()
        {
            var segmentReader = new ArraySegmentReader(new ArraySegment<byte>(new byte[42], 14, 4));
            segmentReader.Position.Should().Be(0);
            segmentReader.BytesRemaining.Should().Be(sizeof(int));
            segmentReader.ReadInt32().Should().Be(0);
            segmentReader.Position.Should().Be(sizeof(int));
            segmentReader.BytesRemaining.Should().Be(0);

            new Action(() => segmentReader.ReadBool()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadByte()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadDouble()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadFloat()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadGuid()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadInt16()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadInt32()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadInt64()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadUInt16()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadUInt32()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadUInt64()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadString()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadShortString()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadByteArray()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadVarlenUInt32()).Should().Throw<IndexOutOfRangeException>();
            new Action(() => segmentReader.ReadVarlenUInt64()).Should().Throw<IndexOutOfRangeException>();
        }
    }
}
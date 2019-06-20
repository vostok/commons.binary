using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Binary.Tests
{
    [TestFixture]
    internal class ByteArrayKey_Tests
    {
        [Test]
        public void Should_works_correctly()
        {
            var random = new Random();

            var expected = new Dictionary<string, Guid>();
            var actual = new Dictionary<ByteArrayKey, Guid>();

            for (var times = 1; times < 100; times++)
            {
                var bytes = new byte[100];
                random.NextBytes(bytes);

                for (var offset = 0; offset < bytes.Length; offset++)
                {
                    for (var length = 1; offset + length <= bytes.Length; length++)
                    {
                        var key = new ByteArrayKey(bytes, offset, length);
                        var part = string.Join(" ", bytes.Skip(offset).Take(length));
                        var value = Guid.NewGuid();

                        if (expected.ContainsKey(part))
                        {
                            actual.TryGetValue(key, out var oldValue).Should().BeTrue();
                            oldValue.Should().Be(expected[part]);

                            expected[part] = value;
                            actual[key] = value;
                        }
                        else
                        {
                            expected.Add(part, value);
                            actual.Add(key, value);
                        }
                    }
                }
            }

            actual.Count.Should().Be(expected.Count);
        }
    }
}
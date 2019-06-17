﻿using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    /// <summary>
    /// Represents comparable segment of byte array.
    /// </summary>
    [PublicAPI]
    internal struct ByteArrayKey
    {
        private readonly byte[] bytes;
        private readonly long offset;
        private readonly long length;
        private readonly int hashCode;

        public ByteArrayKey([NotNull] byte[] bytes, long offset, long length)
        {
            this.bytes = bytes;
            this.offset = offset;
            this.length = length;

            hashCode = CalculateHashCode(bytes, offset, length);
        }

        public override bool Equals(object obj)
        {
            if (obj is ByteArrayKey other)
                return Compare(this, other);
            return false;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        private static unsafe bool Compare(ByteArrayKey first, ByteArrayKey second)
        {
            if (first.length != second.length)
                return false;

            fixed (byte* p1 = first.bytes, p2 = second.bytes)
            {
                byte* x1 = p1, x2 = p2;
                x1 += first.offset;
                x2 += second.offset;

                var l = first.length;
                for (var i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*(long*)x1 != *(long*)x2)
                        return false;
                if ((l & 4) != 0)
                {
                    if (*(int*)x1 != *(int*)x2) return false;
                    x1 += 4;
                    x2 += 4;
                }

                if ((l & 2) != 0)
                {
                    if (*(short*)x1 != *(short*)x2) return false;
                    x1 += 2;
                    x2 += 2;
                }

                if ((l & 1) != 0)
                    if (*x1 != *x2)
                        return false;
                return true;
            }
        }

        private static unsafe int CalculateHashCode(byte[] bytes, long offset, long length)
        {
            var result = 17;

            fixed (byte* p = bytes)
            {
                var x = p;
                x += offset;

                unchecked
                {
                    for (var i = 0; i < length / 8; i++, x += 8)
                        result = result * 31 + (*(long*)x).GetHashCode();

                    if ((length & 4) != 0)
                    {
                        result = result * 33 + (*(int*)x).GetHashCode();
                        x += 4;
                    }

                    if ((length & 2) != 0)
                    {
                        result = result * 37 + (*(short*)x).GetHashCode();
                        x += 2;
                    }

                    if ((length & 1) != 0)
                        result = result * 39 + (*x).GetHashCode();
                }
            }

            return result;
        }
    }
}
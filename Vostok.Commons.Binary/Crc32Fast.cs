using System;
using System.Collections.Generic;

namespace Vostok.Commons.Binary
{
    public static class Crc32Fast
    {
        private const uint Polynome = 0xEDB88320U;
        private const uint InitialXor = 0xFFFFFFFFU;

        private static readonly uint[] Table;
        private static readonly uint[] EvenCache;
        private static readonly uint[] OddCache;
        private static readonly KeyValuePair<int, uint>[] ZeroHashes;

        static Crc32Fast()
        {
            Table = new uint[2048];
            EvenCache = new uint[32];
            OddCache = new uint[32];

            PopulateTable();
            PopulateCaches();

            ZeroHashes = CreatePrecomputedZeroHashes();
        }

        public static uint Compute(byte[] data, uint crc = 0)
        {
            return Compute(data, 0, data.Length, crc);
        }

        public static uint Compute(ArraySegment<byte> data, uint crc = 0)
        {
            return Compute(data.Array, data.Offset, data.Count, crc);
        }

        public static unsafe uint Compute(byte[] data, int offset, int length, uint crc = 0)
        {
            if (crc != 0)
                crc = ReverseBytes(crc);

            crc ^= InitialXor;

            for (; (offset & 7) != 0 && length != 0; length--)
                crc = (crc >> 8) ^ Table[(byte)crc ^ data[offset++]];

            if (length >= 8)
            {
                var to = (length - 8) & ~7;
                length -= to;
                to += offset;

                fixed (byte* dataPointer = data)
                {
                    while (offset != to)
                    {
                        crc ^= *(uint*)(dataPointer + offset);

                        var high = *(uint*)(dataPointer + offset + 4);

                        offset += 8;

                        crc = Table[(byte)crc + 0x700]
                              ^ Table[(byte)(crc >>= 8) + 0x600]
                              ^ Table[(byte)(crc >>= 8) + 0x500]
                              ^ Table[(byte)(crc >> 8) + 0x400]
                              ^ Table[(byte)high + 0x300]
                              ^ Table[(byte)(high >>= 8) + 0x200]
                              ^ Table[(byte)(high >>= 8) + 0x100]
                              ^ Table[(byte)(high >> 8) + 0x000];
                    }
                }
            }

            while (length-- != 0)
                crc = (crc >> 8) ^ Table[(byte)crc ^ data[offset++]];

            crc ^= InitialXor;

            return ReverseBytes(crc);
        }

        public static uint ComputeForZeros(int zerosCount)
        {
            uint crc = 0;

            for (var i = 0; i < ZeroHashes.Length; i++)
            {
                var length = ZeroHashes[i].Key;
                var hash = ZeroHashes[i].Value;

                while (zerosCount >= length)
                {
                    crc = Combine(crc, hash, length);
                    zerosCount -= length;
                }
            }

            if (zerosCount > 0)
                crc = Combine(crc, ComputeForZerosInternal(zerosCount), zerosCount);

            return crc;
        }

        public static uint Combine(uint crc1, uint crc2, int length2)
        {
            if (length2 == 0)
                return crc1;

            if (crc1 == 0)
                return crc2;

            crc1 = ReverseBytes(crc1);
            crc2 = ReverseBytes(crc2);

            var even = CopyArray(EvenCache);
            var odd = CopyArray(OddCache);

            var len2 = (uint)length2;

            do
            {
                GF2MatrixSquare(odd, even);
                if ((len2 & 1) != 0)
                {
                    crc1 = GF2MatrixMultiply(even, crc1);
                }

                len2 >>= 1;

                if (len2 == 0)
                    break;

                GF2MatrixSquare(even, odd);
                if ((len2 & 1) != 0)
                {
                    crc1 = GF2MatrixMultiply(odd, crc1);
                }

                len2 >>= 1;
            } while (len2 != 0);

            return ReverseBytes(crc1 ^ crc2);
        }

        private static void PopulateTable()
        {
            unchecked
            {
                uint i;
                for (i = 0; i < 256; i++)
                {
                    var r = i;
                    for (var j = 0; j < 8; j++)
                        r = (r >> 1) ^ (Polynome & ~((r & 1) - 1));
                    Table[i] = r;
                }

                for (; i < 2048; i++)
                {
                    var r = Table[i - 256];
                    Table[i] = Table[r & 0xFF] ^ (r >> 8);
                }
            }
        }

        private static void PopulateCaches()
        {
            OddCache[0] = Polynome;

            for (var i = 1; i < 32; i++)
            {
                OddCache[i] = 1U << (i - 1);
            }

            GF2MatrixSquare(OddCache, EvenCache);
            GF2MatrixSquare(EvenCache, OddCache);
        }

        private static KeyValuePair<int, uint>[] CreatePrecomputedZeroHashes()
        {
            const int KB = 1024;
            const int MB = 1024 * KB;

            return new[]
            {
                new KeyValuePair<int, uint>(32 * MB, 1157907801U),
                new KeyValuePair<int, uint>(16 * MB, 1252097188U),
                new KeyValuePair<int, uint>(8 * MB, 1170002458U),
                new KeyValuePair<int, uint>(4 * MB, 1782597393U),
                new KeyValuePair<int, uint>(2 * MB, 2122811789U),
                new KeyValuePair<int, uint>(1 * MB, 485111975U),
                new KeyValuePair<int, uint>(512 * KB, 2886362741U),
                new KeyValuePair<int, uint>(256 * KB, 585764578U),
                new KeyValuePair<int, uint>(128 * KB, 3452823678U),
                new KeyValuePair<int, uint>(64 * KB, 3951990743U),
                new KeyValuePair<int, uint>(32 * KB, 2801540865U),
                new KeyValuePair<int, uint>(16 * KB, 2261931179U),
                new KeyValuePair<int, uint>(8 * KB, 2493117656U),
                new KeyValuePair<int, uint>(4 * KB, 285220039U),
                new KeyValuePair<int, uint>(2 * KB, 2663049457U)
            };
        }

        private static void GF2MatrixSquare(uint[] matrix, uint[] square)
        {
            for (var i = 0; i < 32; i++)
                square[i] = GF2MatrixMultiply(matrix, matrix[i]);
        }

        private static uint GF2MatrixMultiply(uint[] matrix, uint vector)
        {
            uint result = 0;
            var i = 0;
            while (vector != 0)
            {
                if ((vector & 1) != 0)
                {
                    result ^= matrix[i];
                }

                vector >>= 1;
                i++;
            }

            return result;
        }

        private static uint ComputeForZerosInternal(int zerosCount)
        {
            uint crc = 0;

            crc ^= InitialXor;

            for (var i = 0; i < zerosCount; i++)
            {
                crc = (crc >> 8) ^ Table[(byte)crc];
            }

            crc ^= InitialXor;

            return ReverseBytes(crc);
        }

        private static uint ReverseBytes(uint crc)
        {
            return (crc >> 24)
                   | (crc << 24)
                   | ((crc >> 8) & 0x0000FF00)
                   | ((crc << 8) & 0x00FF0000);
        }

        private static uint[] CopyArray(uint[] array)
        {
            var result = new uint[array.Length];

            Buffer.BlockCopy(array, 0, result, 0, array.Length * sizeof(uint));

            return result;
        }
    }
}
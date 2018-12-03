using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal class EndiannessConverter
    {
        public static readonly Endianness SystemEndianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Convert(short value, Endianness to)
            => to == SystemEndianness ? value : Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Convert(ushort value, Endianness to)
            => to == SystemEndianness ? value : Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Convert(int value, Endianness to)
            => to == SystemEndianness ? value : Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Convert(uint value, Endianness to)
            => to == SystemEndianness ? value : Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Convert(long value, Endianness to)
            => to == SystemEndianness ? value : Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Convert(ulong value, Endianness to)
            => to == SystemEndianness ? value : Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Convert(float value, Endianness to)
            => to == SystemEndianness ? value : Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Convert(double value, Endianness to)
            => to == SystemEndianness ? value : Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe short Swap(short value)
        {
            var returnValue = short.MinValue;

            Swap2((byte*) &value, (byte*) &returnValue);

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ushort Swap(ushort value)
        {
            var returnValue = ushort.MinValue;

            Swap2((byte*) &value, (byte*) &returnValue);

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int Swap(int value)
        {
            var returnValue = int.MinValue;

            Swap4((byte*) &value, (byte*) &returnValue);

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint Swap(uint value)
        {
            var returnValue = uint.MinValue;

            Swap4((byte*) &value, (byte*) &returnValue);

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long Swap(long value)
        {
            var returnValue = long.MinValue;

            Swap8((byte*) &value, (byte*) &returnValue);

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong Swap(ulong value)
        {
            var returnValue = ulong.MinValue;

            Swap8((byte*) &value, (byte*) &returnValue);

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float Swap(float value)
        {
            var returnValue = float.MinValue;

            Swap4((byte*) &value, (byte*) &returnValue);

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double Swap(double value)
        {
            var returnValue = double.MinValue;

            Swap8((byte*) &value, (byte*) &returnValue);

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Swap2(byte* originalBytes, byte* returnBytes)
        {
            returnBytes[0] = originalBytes[1];
            returnBytes[1] = originalBytes[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Swap4(byte* originalBytes, byte* returnBytes)
        {
            returnBytes[0] = originalBytes[3];
            returnBytes[1] = originalBytes[2];
            returnBytes[2] = originalBytes[1];
            returnBytes[3] = originalBytes[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Swap8(byte* originalBytes, byte* returnBytes)
        {
            returnBytes[0] = originalBytes[7];
            returnBytes[1] = originalBytes[6];
            returnBytes[2] = originalBytes[5];
            returnBytes[3] = originalBytes[4];
            returnBytes[4] = originalBytes[3];
            returnBytes[5] = originalBytes[2];
            returnBytes[6] = originalBytes[1];
            returnBytes[7] = originalBytes[0];
        }
    }
}

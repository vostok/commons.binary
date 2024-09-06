using System;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal interface IBinaryWriter
    {
        /// <summary>
        /// <para>Gets or sets position this writer is at.</para>
        /// <para>This allows to "rewind" the writer and fill values that were not known in advance.</para>
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// <para>Gets or sets the endianness used by this <see cref="IBinaryWriter"/> instance when writing numbers.</para>
        /// <para>Should default to <see cref="EndiannessConverter.SystemEndianness"/>.</para>
        /// </summary>
        Endianness Endianness { get; set; }

        /// <summary>
        /// Writes a single given byte value.
        /// </summary>
        void Write(byte value);

        /// <summary>
        /// Writes a byte with value <c>1</c> if given flag is <c>true</c>. Otherwise, writes a byte with value <c>0</c>.
        /// </summary>
        void Write(bool value);

        /// <summary>
        /// <para>Writes given numerical value in its binary representation.</para>
        /// <para>Binary output of this method is affected by currently selected <see cref="Endianness"/>.</para>
        /// </summary>
        void Write(short value);

        /// <inheritdoc cref="Write(short)"/>
        void Write(int value);

        /// <inheritdoc cref="Write(short)"/>
        void Write(long value);

        /// <inheritdoc cref="Write(short)"/>
        void Write(ushort value);

        /// <inheritdoc cref="Write(short)"/>
        void Write(uint value);

        /// <inheritdoc cref="Write(short)"/>
        void Write(ulong value);

        /// <inheritdoc cref="Write(short)"/>
        void Write(float value);

        /// <inheritdoc cref="Write(short)"/>
        void Write(double value);

        void Write(Guid value);

        /// <summary>
        /// <para>Writes given UInt32 value with a variable number of bytes (from 1 to 5) using VLQ encoding (https://en.wikipedia.org/wiki/Variable-length_quantity).</para>
        /// <para>Note that binary output of this method does not depend on currently selected <see cref="Endianness"/> (it's always big-endian).</para>
        /// </summary>
        int WriteVarlen(uint value);

        /// <summary>
        /// <para>Writes given UInt64 value with a variable number of bytes (from 1 to 9) using VLQ encoding (https://en.wikipedia.org/wiki/Variable-length_quantity).</para>
        /// <para>Note that binary output of this method does not depend on currently selected <see cref="Endianness"/> (it's always big-endian).</para>
        /// </summary>
        int WriteVarlen(ulong value);

        /// <summary>
        /// <para>Writes given string <paramref name="value"/> using given <paramref name="encoding"/>.</para>
        /// <para>The value itself is automatically prepended with Int32 length (see <see cref="Write(int)"/>).</para>
        /// </summary>
        void WriteWithLength([NotNull] string value, [NotNull] Encoding encoding);

        /// <summary>
        /// <para>Writes given string <paramref name="value"/> using given <paramref name="encoding"/>.</para>
        /// <para>The value itself is not automatically prepended with length (readers of binary result must know the length from external sources).</para>
        /// </summary>
        void WriteWithoutLength([NotNull] string value, [NotNull] Encoding encoding);

        /// <summary>
        /// <para>Writes given byte array in its entirety.</para>
        /// <para>The value itself is automatically prepended with Int32 length (see <see cref="Write(int)"/>).</para>
        /// </summary>
        void WriteWithLength([NotNull] byte[] value);

        /// <summary>
        /// <para>Writes a portion of given byte array according to given <paramref name="offset"/> and <paramref name="length"/>.</para>
        /// <para>The value itself is automatically prepended with Int32 length (see <see cref="Write(int)"/>).</para>
        /// </summary>
        void WriteWithLength([NotNull] byte[] value, int offset, int length);

        /// <summary>
        /// <para>Writes given byte array in its entirety.</para>
        /// <para>The value itself is not automatically prepended with length (readers of binary result must know the length from external sources).</para>
        /// </summary>
        void WriteWithoutLength([NotNull] byte[] value);

        /// <summary>
        /// <para>Writes a portion of given byte array according to given <paramref name="offset"/> and <paramref name="length"/>.</para>
        /// <para>The value itself is not automatically prepended with length (readers of binary result must know the length from external sources).</para>
        /// </summary>
        void WriteWithoutLength([NotNull] byte[] value, int offset, int length);

#if NET6_0_OR_GREATER
        /// <summary>
        /// <para>Writes given span of bytes as bytes array in its entirety.</para>
        /// <para>The value itself is automatically prepended with Int32 length (see <see cref="Write(int)"/>).</para>
        /// </summary>
        void WriteWithLength(ReadOnlySpan<byte> value);

        /// <summary>
        /// <para>Writes given span of bytes as bytes array in its entirety.</para>
        /// <para>The value itself is not automatically prepended with length (readers of binary result must know the length from external sources).</para>
        /// </summary>
        void WriteWithoutLength(ReadOnlySpan<byte> value);
#endif
    }
}
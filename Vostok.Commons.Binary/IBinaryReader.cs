using System;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal interface IBinaryReader
    {
        /// <summary>
        /// <para>Gets or sets position this reader is at.</para>
        /// <para>This allows to "jump" the reader to skip unwanted data if its length is known.</para>
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// <para>Gets or sets the endianness used by this <see cref="IBinaryReader"/> instance when reading numbers.</para>
        /// <para>Should default to <see cref="EndiannessConverter.SystemEndianness"/>.</para>
        /// </summary>
        Endianness Endianness { get; set; }

        /// <summary>
        /// Reads a single byte.
        /// </summary>
        byte ReadByte();

        /// <summary>
        /// Reads <c>true</c> if next byte has a non-zero value or <c>false</c> otherwise.
        /// </summary>
        bool ReadBool();

        /// <summary>
        /// <para>Reads a numerical value from its binary representation.</para>
        /// <para>Return value of this method is affected by currently selected <see cref="Endianness"/>.</para>
        /// </summary>
        short ReadInt16();

        /// <inheritdoc cref="ReadInt16"/>
        int ReadInt32();

        /// <inheritdoc cref="ReadInt16"/>
        long ReadInt64();

        /// <inheritdoc cref="ReadInt16"/>
        ushort ReadUInt16();

        /// <inheritdoc cref="ReadInt16"/>
        uint ReadUInt32();

        /// <inheritdoc cref="ReadInt16"/>
        ulong ReadUInt64();

        /// <inheritdoc cref="ReadInt16"/>
        float ReadFloat();

        /// <inheritdoc cref="ReadInt16"/>
        double ReadDouble();

        Guid ReadGuid();

        /// <summary>
        /// <para>Reads an UInt32 value represented by a variable number of bytes (from 1 to 5) using VLQ encoding (https://en.wikipedia.org/wiki/Variable-length_quantity).</para>
        /// <para>Note that behaviour of this method does not depend on currently selected <see cref="Endianness"/> (it always assumes big-endian).</para>
        /// </summary>
        uint ReadVarlenUInt32();

        /// <summary>
        /// <para>Reads an UInt64 value represented by a variable number of bytes (from 1 to 9) using VLQ encoding (https://en.wikipedia.org/wiki/Variable-length_quantity).</para>
        /// <para>Note that behaviour of this method does not depend on currently selected <see cref="Endianness"/> (it always assumes big-endian).</para>
        /// </summary>
        ulong ReadVarlenUInt64();

        /// <summary>
        /// <para>Reads a string in given <paramref name="encoding"/>.</para>
        /// <para>Assumes that the value itself is prepended by its Int32 length.</para>
        /// </summary>
        [NotNull]
        string ReadString(Encoding encoding);

        /// <summary>
        /// <para>Reads a byte array.</para>
        /// <para>Assumes that the value itself is prepended by its Int32 length.</para>
        /// </summary>
        [NotNull]
        byte[] ReadByteArray();

        /// <summary>
        /// Reads a byte array of given <paramref name="size"/>.
        /// </summary>
        [NotNull]
        byte[] ReadByteArray(int size);
    }
}
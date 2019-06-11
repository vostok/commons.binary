using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal static class IBinaryWriterExtensions
    {
        #region Endianness

        public static IBinaryWriter EnsureBigEndian(this IBinaryWriter writer)
        {
            if (writer.Endianness != Endianness.Big)
                throw new ArgumentException("Provided binary writer is little endian.", nameof(writer));

            return writer;
        }

        #endregion

        #region Strings with default UTF-8 encoding

        public static void WriteWithLength([NotNull] this IBinaryWriter writer, [NotNull] string value)
        {
            writer.WriteWithLength(value, Encoding.UTF8);
        }

        public static void WriteWithoutLength([NotNull] this IBinaryWriter writer, [NotNull] string value)
        {
            writer.WriteWithoutLength(value, Encoding.UTF8);
        }

        #endregion

        #region Derived primitive types

        public static void Write([NotNull] this IBinaryWriter writer, TimeSpan value)
        {
            writer.Write(value.Ticks);
        }

        public static void Write([NotNull] this IBinaryWriter writer, DateTime value)
        {
            writer.Write(value.ToBinary());
        }

        #endregion

        #region Collections

        public static void WriteCollection<T>(
            [NotNull] this IBinaryWriter writer,
            [NotNull] ICollection<T> items,
            [NotNull] Action<IBinaryWriter, T> writeSingleValue)
        {
            writer.Write(items.Count);

            foreach (var item in items)
            {
                writeSingleValue(writer, item);
            }
        }

        public static void WriteDictionary<TKey, TValue>(
            [NotNull] this IBinaryWriter writer,
            [NotNull] IReadOnlyDictionary<TKey, TValue> items,
            [NotNull] Action<IBinaryWriter, TKey> writeSingleKey,
            [NotNull] Action<IBinaryWriter, TValue> writeSingleValue)
        {
            writer.Write(items.Count);

            foreach (var pair in items)
            {
                writeSingleKey(writer, pair.Key);
                writeSingleValue(writer, pair.Value);
            }
        }

        #endregion

        #region Nullables

        public static void WriteNullable<T>([NotNull] this IBinaryWriter writer, [CanBeNull] T value, [NotNull] Action<IBinaryWriter, T> writeNonNullValue)
            where T : class
        {
            writer.Write(value != null);

            if (value != null)
                writeNonNullValue(writer, value);
        }

        public static void WriteNullable<T>([NotNull] this IBinaryWriter writer, [CanBeNull] T? value, [NotNull] Action<IBinaryWriter, T> writeNonNullValue)
            where T : struct
        {
            writer.Write(value.HasValue);

            if (value != null)
                writeNonNullValue(writer, value.Value);
        }

        #endregion

        #region Jump

        public static JumpToken JumpTo([NotNull] this IBinaryWriter writer, long position)
        {
            var originalPosition = writer.Position;

            writer.Position = position;

            return new JumpToken(writer, originalPosition);
        }

        public struct JumpToken : IDisposable
        {
            private readonly IBinaryWriter writer;
            private readonly long position;

            public JumpToken(IBinaryWriter writer, long position)
            {
                this.writer = writer;
                this.position = position;
            }

            public void Dispose()
            {
                writer.Position = position;
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal static class IBinaryReaderExtensions
    {
        #region Endianness

        public static IBinaryReader EnsureBigEndian(this IBinaryReader reader)
        {
            if (reader.Endianness != Endianness.Big)
                throw new ArgumentException("Provided binary reader is little endian.", nameof(reader));

            return reader;
        }

        #endregion

        #region Strings with default UTF-8 encoding

        [NotNull]
        public static string ReadString([NotNull] this IBinaryReader reader)
        {
            return reader.ReadString(Encoding.UTF8);
        }

        [NotNull]
        public static string ReadShortString([NotNull] this IBinaryReader reader)
        {
            return reader.ReadShortString(Encoding.UTF8);
        }

        #endregion

        #region Derived primitive types

        public static TimeSpan ReadTimeSpan([NotNull] this IBinaryReader reader)
        {
            return TimeSpan.FromTicks(reader.ReadInt64());
        }

        public static DateTime ReadDateTime([NotNull] this IBinaryReader reader)
        {
            return DateTime.FromBinary(reader.ReadInt64());
        }

        #endregion

        #region Collections

        [NotNull]
        public static T[] ReadArray<T>([NotNull] this IBinaryReader reader, [NotNull] Func<IBinaryReader, T> readSingleValue)
        {
            var count = reader.ReadInt32();
            var result = new T[count];

            for (var i = 0; i < count; i++)
            {
                result[i] = readSingleValue(reader);
            }

            return result;
        }

        [NotNull]
        public static List<T> ReadList<T>([NotNull] this IBinaryReader reader, [NotNull] Func<IBinaryReader, T> readSingleValue)
        {
            var count = reader.ReadInt32();
            var result = new List<T>(count);

            for (var i = 0; i < count; i++)
            {
                result.Add(readSingleValue(reader));
            }

            return result;
        }

        [NotNull]
        public static HashSet<T> ReadSet<T>([NotNull] this IBinaryReader reader, [NotNull] Func<IBinaryReader, T> readSingleValue)
        {
            var count = reader.ReadInt32();
            var result = new HashSet<T>();

            for (var i = 0; i < count; i++)
            {
                result.Add(readSingleValue(reader));
            }

            return result;
        }

        [NotNull]
        public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(
            [NotNull] this IBinaryReader reader,
            [NotNull] Func<IBinaryReader, TKey> readSingleKey,
            [NotNull] Func<IBinaryReader, TValue> readSingleValue)
        {
            var count = reader.ReadInt32();
            var result = new Dictionary<TKey, TValue>(count);

            for (var i = 0; i < count; i++)
            {
                var key = readSingleKey(reader);
                var value = readSingleValue(reader);

                result[key] = value;
            }

            return result;
        }

        #endregion

        #region Nullables

        [CanBeNull]
        public static T ReadNullable<T>([NotNull] this IBinaryReader reader, [NotNull] Func<IBinaryReader, T> readNonNullValue)
            where T : class
        {
            return reader.ReadBool() ? readNonNullValue(reader) : null;
        }

        [CanBeNull]
        public static T? ReadNullableStruct<T>([NotNull] this IBinaryReader reader, [NotNull] Func<IBinaryReader, T> readNonNullValue)
            where T : struct
        {
            return reader.ReadBool() ? readNonNullValue(reader) : (T?)null;
        }

        #endregion

        #region Jump

        public static JumpToken JumpTo([NotNull] this IBinaryReader reader, long position)
        {
            var originalPosition = reader.Position;

            reader.Position = position;

            return new JumpToken(reader, originalPosition);
        }

        public struct JumpToken : IDisposable
        {
            public readonly long Position;
            private readonly IBinaryReader reader;

            public JumpToken(IBinaryReader reader, long position)
            {
                this.reader = reader;
                Position = position;
            }

            public void Dispose()
            {
                reader.Position = Position;
            }
        }

        #endregion
    }
}
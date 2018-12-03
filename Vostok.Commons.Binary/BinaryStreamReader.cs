﻿using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Binary
{
    [PublicAPI]
    internal class BinaryStreamReader : IBinaryReader
    {
        private const int BufferSize = 16;

        private readonly Stream stream;
        private readonly BinaryBufferReader buffer;

        public BinaryStreamReader(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new AggregateException($"Input stream of type '{stream.GetType().Name}' was not readable.");

            buffer = new BinaryBufferReader(new byte[BufferSize], 0);
        }

        public long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        public Endianness Endianness
        {
            get => buffer.Endianness;
            set => buffer.Endianness = value;
        }

        public byte ReadByte()
        {
            LoadIntoBufferExactly(sizeof(byte));

            return buffer.ReadByte();
        }

        public bool ReadBool() => ReadByte() != 0;

        public short ReadInt16()
        {
            LoadIntoBufferExactly(sizeof(short));

            return buffer.ReadInt16();
        }

        public int ReadInt32()
        {
            LoadIntoBufferExactly(sizeof(int));

            return buffer.ReadInt32();
        }

        public long ReadInt64()
        {
            LoadIntoBufferExactly(sizeof(long));

            return buffer.ReadInt64();
        }

        public ushort ReadUInt16()
        {
            LoadIntoBufferExactly(sizeof(ushort));

            return buffer.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            LoadIntoBufferExactly(sizeof(uint));

            return buffer.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            LoadIntoBufferExactly(sizeof(ulong));

            return buffer.ReadUInt64();
        }

        public float ReadFloat()
        {
            LoadIntoBufferExactly(sizeof(float));

            return buffer.ReadFloat();
        }

        public double ReadDouble()
        {
            LoadIntoBufferExactly(sizeof(double));

            return buffer.ReadDouble();
        }

        public Guid ReadGuid()
        {
            LoadIntoBufferExactly(16);

            return buffer.ReadGuid();
        }

        public uint ReadVarlenUInt32()
        {
            LoadIntoBufferAtMost(5);

            return buffer.ReadVarlenUInt32();
        }

        public ulong ReadVarlenUInt64()
        {
            LoadIntoBufferAtMost(9);

            return buffer.ReadVarlenUInt64();
        }

        public string ReadString(Encoding encoding)
        {
            var size = ReadInt32();

            var stringBuffer = size > BufferSize ? new byte[size] : buffer.Buffer;

            ReadFromStreamExactly(stringBuffer, 0, size);

            return encoding.GetString(stringBuffer, 0, size);
        }

        public byte[] ReadByteArray()
        {
            return ReadByteArray(ReadInt32());
        }

        public byte[] ReadByteArray(int size)
        {
            var result = new byte[size];

            ReadFromStreamExactly(result, 0, size);

            return result;
        }

        private void LoadIntoBufferExactly(int size)
        {
            ReadFromStreamExactly(buffer.Buffer, 0, size);

            buffer.Position = 0;
        }

        private void LoadIntoBufferAtMost(int size)
        {
            ReadFromStreamExactly(buffer.Buffer, 0, size);

            buffer.Position = 0;
        }

        private void ReadFromStreamExactly(byte[] destination, int offset, int size)
        {
            var totalBytesRead = 0;

            while (totalBytesRead < size)
            {
                var bytesRead = stream.Read(destination, offset + totalBytesRead, size - totalBytesRead);
                if (bytesRead == 0)
                    throw new Exception($"Can't read from stream: expected to read {size} bytes, but read only {totalBytesRead} bytes.");

                totalBytesRead += bytesRead;
            }
        }

        private void ReadFromStreamAtMost(byte[] destination, int offset, int sizeLimit)
        {
            var totalBytesRead = 0;

            while (totalBytesRead < sizeLimit)
            {
                var bytesRead = stream.Read(destination, offset + totalBytesRead, sizeLimit - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (totalBytesRead > 0)
                        break;

                    throw new Exception("Can't read from stream: expected to read at least a single byte, but read nothing.");
                }

                totalBytesRead += bytesRead;
            }
        }
    }
}
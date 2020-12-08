using System;
using System.IO;
using System.Text;

namespace QueenOfHearts.CoreLibrary.Serialization
{
    public unsafe class BinaryDeserializer : IBinaryDeserializer
    {
        private readonly Stream _inStream;
        private readonly byte[] _simpleTypeBuffer = new byte[16];

        public BinaryDeserializer(Stream inStream)
        {
            if (!inStream.CanRead)
                throw new ArgumentException("Stream should be readable", nameof(inStream));
            _inStream = inStream;
        }

        public bool CanRead => _inStream.Length > _inStream.Position;
        public long Length => _inStream.Length;
        
        public long Position
        {
            get => _inStream.Position;
            set => _inStream.Position = value;
        }

        public int ReadInt32()
        {
            fixed (byte* ptr = ReadFromStream(sizeof(int)))
            {
                return *(int*) ptr;
            }
        }

        public string ReadString()
        {
            return ReadString(Encoding.UTF32);
        }

        public Guid ReadGuid()
        {
            fixed (byte* ptr = ReadFromStream(sizeof(Guid)))
            {
                return *(Guid*) ptr;
            }
        }

        public void Dispose()
        {
            _inStream?.Dispose();
        }

        public bool CanReadSize(int size)
        {
            return _inStream.Length > _inStream.Position + size;
        }

        public long ReadInt64()
        {
            fixed (byte* ptr = ReadFromStream(sizeof(long)))
            {
                return *(long*) ptr;
            }
        }

        public string ReadString(int length, Encoding encoding)
        {
            var result = length > _simpleTypeBuffer.Length ? new byte[length] : _simpleTypeBuffer;

            ReadFromStreamInternal(result, 0, length);

            return encoding.GetString(result, 0, length);
        }

        private string ReadString(Encoding encoding)
        {
            var length = ReadInt32();
            return ReadString(length, encoding);
        }

        private byte[] ReadFromStream(int size)
        {
            ReadFromStreamInternal(_simpleTypeBuffer, 0, size);
            return _simpleTypeBuffer;
        }

        private void ReadFromStreamInternal(byte[] dest, int offset, int count)
        {
            var totalBytesRead = 0;

            while (totalBytesRead != count)
            {
                var bytesRead = _inStream.Read(dest, offset + totalBytesRead, count - totalBytesRead);

                if (bytesRead == 0)
                    throw new Exception(
                        $"Can't read from stream, expected to read {count - totalBytesRead} bytes, but read 0 bytes.");

                totalBytesRead += bytesRead;
            }
        }
    }
}
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QueenOfHearts.CoreLibrary.Serialization
{
    public sealed unsafe class BinarySerializer : IBinarySerializer
    {
        private readonly byte[] _buffer = new byte[16];

        private readonly Stream _outputStream;

        public BinarySerializer(Stream outputStream)
        {
            if (!outputStream.CanWrite)
                throw new ArgumentException("Stream should be writeable", nameof(outputStream));
            _outputStream = outputStream;
        }

        public long Position
        {
            get => _outputStream.Position;
            set => _outputStream.Position = value;
        }

        public void Write(int i)
        {
            fixed (byte* ptr = &_buffer[0])
            {
                *(int*) ptr = i;
            }

            WriteToStream(_buffer, 0, sizeof(int));
        }

        public void Write(Guid g)
        {
            fixed (byte* ptr = &_buffer[0])
            {
                *(Guid*) ptr = g;
            }

            WriteToStream(_buffer, 0, sizeof(Guid));
        }

        public void Write(string str)
        {
            Write(str, Encoding.UTF32);
        }

        public Task FlushAsync()
        {
            return _outputStream.FlushAsync();
        }

        public void Dispose()
        {
            _outputStream?.Dispose();
        }

        public void Write(long i)
        {
            fixed (byte* ptr = &_buffer[0])
            {
                *(long*) ptr = i;
            }

            WriteToStream(_buffer, 0, sizeof(long));
        }

        public void Write(byte[] bb)
        {
            Write(bb, 0, bb.Length);
        }

        public void Write(byte[] bb, int offset, int length)
        {
            Write(length);
            WriteToStream(bb, offset, length);
        }

        private void Write(string str, Encoding encoding)
        {
            Write(encoding.GetBytes(str));
        }

        private void WriteToStream(byte[] bytes, int offset, int count)
        {
            _outputStream.Write(bytes, offset, count);
        }
    }
}
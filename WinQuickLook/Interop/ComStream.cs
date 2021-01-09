using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace WinQuickLook.Interop
{
    public class ComStream : IStream
    {
        internal ComStream(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        private readonly Stream _stream;

        public void Read(byte[] buffer, int bufferSize, IntPtr bytesReadPtr)
        {
            int bytesRead = _stream.Read(buffer, 0, bufferSize);

            if (bytesReadPtr != IntPtr.Zero)
            {
                Marshal.WriteInt32(bytesReadPtr, bytesRead);
            }
        }

        public void Seek(long offset, int origin, IntPtr newPositionPtr)
        {
            SeekOrigin seekOrigin;

            switch (origin)
            {
                case 0x0:
                    seekOrigin = SeekOrigin.Begin;
                    break;
                case 0x1:
                    seekOrigin = SeekOrigin.Current;
                    break;
                case 0x2:
                    seekOrigin = SeekOrigin.End;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }

            long position = _stream.Seek(offset, seekOrigin);

            if (newPositionPtr != IntPtr.Zero)
            {
                Marshal.WriteInt64(newPositionPtr, position);
            }
        }

        public void SetSize(long libNewSize)
        {
            _stream.SetLength(libNewSize);
        }

        public void Stat(out STATSTG streamStats, int grfStatFlag)
        {
            streamStats = new STATSTG
            {
                type = 2,
                cbSize = _stream.Length,
                grfMode = 0
            };

            if (_stream.CanRead && _stream.CanWrite)
            {
                streamStats.grfMode |= 0x00000002;
            }
            else if (_stream.CanRead)
            {
                streamStats.grfMode |= 0x00000000;
            }
            else if (_stream.CanWrite)
            {
                streamStats.grfMode |= 0x00000001;
            }
            else
            {
                throw new IOException();
            }
        }

        public void Write(byte[] buffer, int bufferSize, IntPtr bytesWrittenPtr)
        {
            _stream.Write(buffer, 0, bufferSize);

            if (bytesWrittenPtr != IntPtr.Zero)
            {
                Marshal.WriteInt32(bytesWrittenPtr, bufferSize);
            }
        }

        public void Clone(out IStream streamCopy)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(IStream targetStream, long bufferSize, IntPtr buffer, IntPtr bytesWrittenPtr)
        {
            throw new NotSupportedException();
        }

        public void Commit(int flags)
        {
            throw new NotSupportedException();
        }

        public void LockRegion(long offset, long byteCount, int lockType)
        {
            throw new NotSupportedException();
        }

        public void Revert()
        {
            throw new NotSupportedException();
        }

        public void UnlockRegion(long offset, long byteCount, int lockType)
        {
            throw new NotSupportedException();
        }
    }
}

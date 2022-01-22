﻿using System;
using System.IO;
using System.Runtime.InteropServices;

using Windows.Win32.System.Com;

namespace WinQuickLook.CsWin32;

public class ComStream : IStream
{
    public ComStream(Stream stream)
    {
        _baseStream = stream;
    }

    private readonly Stream _baseStream;

    public unsafe void Read(void* pv, uint cb, uint* pcbRead)
    {
        var buffer = new Span<byte>(pv, (int)cb);

        var bytesRead = _baseStream.Read(buffer);

        if (pcbRead is not null)
        {
            Marshal.WriteInt32(new IntPtr(pcbRead), bytesRead);
        }
    }

    public unsafe void Write(void* pv, uint cb, uint* pcbWritten)
    {
        var buffer = new ReadOnlySpan<byte>(pv, (int)cb);

        _baseStream.Write(buffer);

        if (pcbWritten is not null)
        {
            Marshal.WriteInt32(new IntPtr(pcbWritten), buffer.Length);
        }
    }

    public unsafe void Seek(long dlibMove, STREAM_SEEK dwOrigin, ulong* plibNewPosition = default)
    {
        var seekOrigin = dwOrigin switch
        {
            STREAM_SEEK.STREAM_SEEK_SET => SeekOrigin.Begin,
            STREAM_SEEK.STREAM_SEEK_CUR => SeekOrigin.Current,
            STREAM_SEEK.STREAM_SEEK_END => SeekOrigin.End,
            _ => throw new ArgumentOutOfRangeException(nameof(dwOrigin))
        };

        var newPosition = _baseStream.Seek(dlibMove, seekOrigin);

        if (plibNewPosition is not null)
        {
            Marshal.WriteInt64(new IntPtr(plibNewPosition), newPosition);
        }
    }

    public void SetSize(ulong libNewSize) => _baseStream.SetLength((long)libNewSize);

    public unsafe void CopyTo(IStream pstm, ulong cb, ulong* pcbRead = default, ulong* pcbWritten = default) => throw new NotSupportedException();

    public void Commit(uint grfCommitFlags) => throw new NotSupportedException();

    public void Revert() => throw new NotSupportedException();

    public void LockRegion(ulong libOffset, ulong cb, uint dwLockType) => throw new NotSupportedException();

    public void UnlockRegion(ulong libOffset, ulong cb, uint dwLockType) => throw new NotSupportedException();

    public unsafe void Stat(STATSTG* pstatstg, uint grfStatFlag)
    {
        var statStg = new STATSTG
        {
            type = 2,
            cbSize = (ulong)_baseStream.Length
        };

        Marshal.StructureToPtr(statStg, new IntPtr(pstatstg), false);
    }

    public void Clone(out IStream ppstm) => throw new NotSupportedException();
}

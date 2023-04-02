﻿using System;
using System.IO;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace Windows.Win32;

public class ComInteropStream : IStream
{
    public ComInteropStream(Stream baseStream)
    {
        ArgumentNullException.ThrowIfNull(baseStream);

        _baseStream = baseStream;
    }

    private readonly Stream _baseStream;

    public unsafe HRESULT Read(void* pv, uint cb, out uint pcbRead)
    {
        var buffer = new Span<byte>(pv, (int)cb);

        pcbRead = (uint)_baseStream.Read(buffer);

        return new HRESULT();
    }

    public unsafe HRESULT Write(void* pv, uint cb, out uint pcbWritten)
    {
        var buffer = new ReadOnlySpan<byte>(pv, (int)cb);

        _baseStream.Write(buffer);

        pcbWritten = (uint)buffer.Length;

        return new HRESULT();
    }

    public HRESULT Seek(long dlibMove, SeekOrigin dwOrigin, out ulong plibNewPosition)
    {
        plibNewPosition = (ulong)_baseStream.Seek(dlibMove, dwOrigin);

        return new HRESULT();
    }

    public HRESULT SetSize(ulong libNewSize)
    {
        _baseStream.SetLength((long)libNewSize);

        return new HRESULT();
    }

    public HRESULT CopyTo(IStream pstm, ulong cb, out ulong pcbRead, out ulong pcbWritten) => throw new NotSupportedException();

    public HRESULT Commit(STGC grfCommitFlags) => throw new NotSupportedException();

    public HRESULT Revert() => throw new NotSupportedException();

    public HRESULT LockRegion(ulong libOffset, ulong cb, LOCKTYPE dwLockType) => throw new NotSupportedException();

    public HRESULT UnlockRegion(ulong libOffset, ulong cb, uint dwLockType) => throw new NotSupportedException();

    public unsafe HRESULT Stat(STATSTG* pstatstg, STATFLAG grfStatFlag)
    {
        var statStg = new STATSTG
        {
            type = 2,
            cbSize = (ulong)_baseStream.Length,
            grfMode = 0
        };

        Marshal.StructureToPtr(statStg, new nint(pstatstg), false);

        return new HRESULT();
    }

    public HRESULT Clone(out IStream ppstm) => throw new NotSupportedException();
}

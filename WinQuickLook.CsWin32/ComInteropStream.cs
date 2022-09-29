using System;
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

    public unsafe HRESULT Read(void* pv, uint cb, uint* pcbRead)
    {
        var buffer = new Span<byte>(pv, (int)cb);

        var bytesRead = _baseStream.Read(buffer);

        if (pcbRead is not null)
        {
            Marshal.WriteInt32(new IntPtr(pcbRead), bytesRead);
        }

        return new HRESULT();
    }

    public unsafe HRESULT Write(void* pv, uint cb, uint* pcbWritten)
    {
        var buffer = new ReadOnlySpan<byte>(pv, (int)cb);

        _baseStream.Write(buffer);

        if (pcbWritten is not null)
        {
            Marshal.WriteInt32(new IntPtr(pcbWritten), buffer.Length);
        }

        return new HRESULT();
    }

    public unsafe void Seek(long dlibMove, SeekOrigin dwOrigin, [Optional] ulong* plibNewPosition)
    {
        var newPosition = _baseStream.Seek(dlibMove, dwOrigin);

        if (plibNewPosition is not null)
        {
            Marshal.WriteInt64(new IntPtr(plibNewPosition), newPosition);
        }
    }

    public void SetSize(ulong libNewSize) => _baseStream.SetLength((long)libNewSize);

    public unsafe void CopyTo(IStream pstm, ulong cb, ulong* pcbRead = default, ulong* pcbWritten = default) => throw new NotSupportedException();

    public void Commit(STGC grfCommitFlags) => throw new NotSupportedException();

    public void Revert() => throw new NotSupportedException();

    public void LockRegion(ulong libOffset, ulong cb, LOCKTYPE dwLockType) => throw new NotSupportedException();

    public void UnlockRegion(ulong libOffset, ulong cb, uint dwLockType) => throw new NotSupportedException();

    public unsafe void Stat(STATSTG* pstatstg, STATFLAG grfStatFlag)
    {
        var statStg = new STATSTG
        {
            type = 2,
            cbSize = (ulong)_baseStream.Length,
            grfMode = 0
        };

        Marshal.StructureToPtr(statStg, new IntPtr(pstatstg), false);
    }

    public void Clone(out IStream ppstm) => throw new NotSupportedException();
}

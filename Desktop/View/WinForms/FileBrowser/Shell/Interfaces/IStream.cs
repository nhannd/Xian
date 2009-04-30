#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ClearCanvas.Desktop.View.WinForms.FileBrowser.ShellDll
{
    [ComImport]
    [Guid("0000000c-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IStream
    {
        // Reads a specified number of bytes from the stream object into memory 
        // starting at the current seek pointer
        [PreserveSig]
        Int32 Read(
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] pv,
            int cb,
            IntPtr pcbRead);

        // Writes a specified number of bytes into the stream object starting at 
        // the current seek pointer
        [PreserveSig]
        Int32 Write(
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] pv,
            int cb,
            IntPtr pcbWritten);

        // Changes the seek pointer to a new location relative to the beginning 
        // of the stream, the end of the stream, or the current seek pointer
        [PreserveSig]
        Int32 Seek(
            long dlibMove,
            SeekOrigin dwOrigin,
            IntPtr plibNewPosition);

        // Changes the size of the stream object
        [PreserveSig]
        Int32 SetSize(
            long libNewSize);

        // Copies a specified number of bytes from the current seek pointer in 
        // the stream to the current seek pointer in another stream
        [PreserveSig]
        Int32 CopyTo(
            IStream pstm,
            long cb,
            IntPtr pcbRead,
            IntPtr pcbWritten);

        // Ensures that any changes made to a stream object open in transacted 
        // mode are reflected in the parent storage object
        [PreserveSig]
        Int32 Commit(
            ShellAPI.STGC grfCommitFlags);

        // Discards all changes that have been made to a transacted stream since 
        // the last call to IStream::Commit
        [PreserveSig]
        Int32 Revert();

        // Restricts access to a specified range of bytes in the stream. Supporting 
        // this functionality is optional since some file systems do not provide it
        [PreserveSig]
        Int32 LockRegion(
            long libOffset,
            long cb,
            ShellAPI.LOCKTYPE dwLockType);

        // Removes the access restriction on a range of bytes previously restricted 
        // with IStream::LockRegion
        [PreserveSig]
        Int32 UnlockRegion(
            long libOffset,
            long cb,
            ShellAPI.LOCKTYPE dwLockType);

        // Retrieves the STATSTG structure for this stream
        [PreserveSig]
        Int32 Stat(
            out ShellAPI.STATSTG pstatstg,
            ShellAPI.STATFLAG grfStatFlag);

        // Creates a new stream object that references the same bytes as the original 
        // stream but provides a separate seek pointer to those bytes
        [PreserveSig]
        Int32 Clone(
            out IntPtr ppstm);
    }
}

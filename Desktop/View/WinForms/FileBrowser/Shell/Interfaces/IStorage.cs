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
using System.Runtime.InteropServices;

namespace ClearCanvas.Desktop.View.WinForms.FileBrowser.ShellDll
{
    [ComImport]
    [Guid("0000000b-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IStorage
    {
        [PreserveSig]
        Int32 CreateStream(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsName,
            ShellAPI.STGM grfMode,
            int reserved1,
            int reserved2,
            out IntPtr ppstm);

        [PreserveSig]
        Int32 OpenStream(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsName,
            IntPtr reserved1,
            ShellAPI.STGM grfMode,
            int reserved2,
            out IntPtr ppstm);

        [PreserveSig]
        Int32 CreateStorage(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsName,
            ShellAPI.STGM grfMode,
            int reserved1,
            int reserved2,
            out IntPtr ppstg);

        [PreserveSig]
        Int32 OpenStorage(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsName,
            IStorage pstgPriority,
            ShellAPI.STGM grfMode,
            IntPtr snbExclude,
            int reserved,
            out IntPtr ppstg);

        [PreserveSig]
        Int32 CopyTo(
            int ciidExclude,
            ref Guid rgiidExclude,
            IntPtr snbExclude,
            IStorage pstgDest);

        [PreserveSig]
        Int32 MoveElementTo(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsName,
            IStorage pstgDest,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsNewName,
            ShellAPI.STGMOVE grfFlags);

        [PreserveSig]
        Int32 Commit(
            ShellAPI.STGC grfCommitFlags);

        [PreserveSig]
        Int32 Revert();

        [PreserveSig]
        Int32 EnumElements(
            int reserved1,
            IntPtr reserved2,
            int reserved3,
            out IntPtr ppenum);

        [PreserveSig]
        Int32 DestroyElement(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsName);

        [PreserveSig]
        Int32 RenameElement(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsOldName,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsNewName);

        [PreserveSig]
        Int32 SetElementTimes(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwcsName,
            ShellAPI.FILETIME pctime,
            ShellAPI.FILETIME patime,
            ShellAPI.FILETIME pmtime);

        [PreserveSig]
        Int32 SetClass(
            ref Guid clsid);

        [PreserveSig]
        Int32 SetStateBits(
            int grfStateBits,
            int grfMask);

        [PreserveSig]
        Int32 Stat(
            out ShellAPI.STATSTG pstatstg,
            ShellAPI.STATFLAG grfStatFlag);
    }
}

#region License (non-CC)

// Copyright (c) 2007, Paul Wheeler
//
// This work is licensed under a Creative Commons Attribution 3.0 Unported License.
// For the complete license, see http://creativecommons.org/licenses/by/3.0/
// Or, you may send a letter to: 
//    Creative Commons
//    171 Second Street, Suite 300
//    San Francisco, California, 94105, USA.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HttpNamespaceManager.Lib.AccessControl
{
    internal static class Util
    {
        internal static string GetErrorMessage(UInt32 errorCode)
        {
            UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
            UInt32 FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
            UInt32 FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

            UInt32 dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;

            IntPtr source = new IntPtr();

            string msgBuffer = "";

            UInt32 retVal = FormatMessage(dwFlags, source, errorCode, 0, ref msgBuffer, 512, null);

            return msgBuffer.ToString();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern UInt32 FormatMessage(UInt32 dwFlags, IntPtr lpSource, UInt32 dwMessageId, UInt32 dwLanguageId, [MarshalAs(UnmanagedType.LPTStr)] ref string lpBuffer, int nSize, IntPtr[] Arguments);

        /*
         * DWORD GetLastError(void);
         */
        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();

        /*
         * HLOCAL LocalAlloc(
         *     UINT uFlags,
         *     SIZE_T uBytes
         * );
         */
        [DllImport("Kernel32.dll")]
        internal static extern IntPtr LocalAlloc(LocalAllocFlags uFlags, uint uBytes);

        /*
         * HLOCAL LocalFree(
         *     HLOCAL hMem
         * );
         */
        [DllImport("Kernel32.dll")]
        internal static extern IntPtr LocalFree(IntPtr hMem);
    }

    [Flags]
    internal enum LocalAllocFlags
    {
        Fixed = 0x00,
        Moveable = 0x20,
        ZeroInit = 0x40
    }
}

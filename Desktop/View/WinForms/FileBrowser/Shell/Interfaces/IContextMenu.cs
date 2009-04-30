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
using System.Text;

namespace ClearCanvas.Desktop.View.WinForms.FileBrowser.ShellDll
{
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("000214e4-0000-0000-c000-000000000046")]
    public interface IContextMenu
    {
        // Adds commands to a shortcut menu
        [PreserveSig()]
        Int32 QueryContextMenu(
            IntPtr hmenu,
            uint iMenu, 
            uint idCmdFirst, 
            uint idCmdLast, 
            ShellAPI.CMF uFlags);

        // Carries out the command associated with a shortcut menu item
        [PreserveSig()]
        Int32 InvokeCommand(
            ref ShellAPI.CMINVOKECOMMANDINFOEX info);

        // Retrieves information about a shortcut menu command, 
        // including the help string and the language-independent, 
        // or canonical, name for the command
        [PreserveSig()]
        Int32 GetCommandString(
            uint idcmd,
            ShellAPI.GCS uflags, 
            uint reserved,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] commandstring,
            int cch);
    }
    
    [ComImport, Guid("000214f4-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IContextMenu2
    {
        // Adds commands to a shortcut menu
        [PreserveSig()]
        Int32 QueryContextMenu(
            IntPtr hmenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            ShellAPI.CMF uFlags);

        // Carries out the command associated with a shortcut menu item
        [PreserveSig()]
        Int32 InvokeCommand(
            ref ShellAPI.CMINVOKECOMMANDINFOEX info);

        // Retrieves information about a shortcut menu command, 
        // including the help string and the language-independent, 
        // or canonical, name for the command
        [PreserveSig()]
        Int32 GetCommandString(
            uint idcmd,
            ShellAPI.GCS uflags,
            uint reserved,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder commandstring,
            int cch);

        // Allows client objects of the IContextMenu interface to 
        // handle messages associated with owner-drawn menu items
        [PreserveSig]
        Int32 HandleMenuMsg(
            uint uMsg, 
            IntPtr wParam, 
            IntPtr lParam);
    }

    [ComImport, Guid("bcfce0a0-ec17-11d0-8d10-00a0c90f2719")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IContextMenu3
    {
        // Adds commands to a shortcut menu
        [PreserveSig()]
        Int32 QueryContextMenu(
            IntPtr hmenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            ShellAPI.CMF uFlags);

        // Carries out the command associated with a shortcut menu item
        [PreserveSig()]
        Int32 InvokeCommand(
            ref ShellAPI.CMINVOKECOMMANDINFOEX info);

        // Retrieves information about a shortcut menu command, 
        // including the help string and the language-independent, 
        // or canonical, name for the command
        [PreserveSig()]
        Int32 GetCommandString(
            uint idcmd,
            ShellAPI.GCS uflags,
            uint reserved,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder commandstring,
            int cch);

        // Allows client objects of the IContextMenu interface to 
        // handle messages associated with owner-drawn menu items
        [PreserveSig]
        Int32 HandleMenuMsg(
            uint uMsg, 
            IntPtr wParam, 
            IntPtr lParam);

        // Allows client objects of the IContextMenu3 interface to 
        // handle messages associated with owner-drawn menu items
        [PreserveSig]
        Int32 HandleMenuMsg2(
            uint uMsg, 
            IntPtr wParam, 
            IntPtr lParam, 
            IntPtr plResult);
    }
}

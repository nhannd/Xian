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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms.FileBrowser.ShellDll
{
    [ComImport]
    [GuidAttribute("4657278B-411B-11d2-839A-00C04FD918D0")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDropTargetHelper
    {
        // Notifies the drag-image manager that the drop target's IDropTarget::DragEnter method has been called
        [PreserveSig]
        Int32 DragEnter(      
            IntPtr hwndTarget,
            IntPtr pDataObject,
            ref ShellAPI.POINT ppt,
            DragDropEffects dwEffect);

        // Notifies the drag-image manager that the drop target's IDropTarget::DragLeave method has been called
        [PreserveSig]
        Int32 DragLeave();

        // Notifies the drag-image manager that the drop target's IDropTarget::DragOver method has been called
        [PreserveSig]
        Int32 DragOver(
            ref ShellAPI.POINT ppt,
            DragDropEffects dwEffect);

        // Notifies the drag-image manager that the drop target's IDropTarget::Drop method has been called
        [PreserveSig]
        Int32 Drop(
            IntPtr pDataObject,
            ref ShellAPI.POINT ppt,
            DragDropEffects dwEffect);

        // Notifies the drag-image manager to show or hide the drag image
        [PreserveSig]
        Int32 Show(
            bool fShow);
    }
}

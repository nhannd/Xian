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
    [Guid("0000010e-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataObject
    {
        // Renders the data described in a FORMATETC structure 
        // and transfers it through the STGMEDIUM structure
        [PreserveSig]
        Int32 GetData(
            ref ShellAPI.FORMATETC pformatetcIn,
            ref ShellAPI.STGMEDIUM pmedium);

        // Renders the data described in a FORMATETC structure 
        // and transfers it through the STGMEDIUM structure allocated by the caller
        [PreserveSig]
        Int32 GetDataHere(
            ref ShellAPI.FORMATETC pformatetcIn,
            ref ShellAPI.STGMEDIUM pmedium);

        // Determines whether the data object is capable of 
        // rendering the data described in the FORMATETC structure
        [PreserveSig]
        Int32 QueryGetData(
            ref ShellAPI.FORMATETC pformatetc);

        // Provides a potentially different but logically equivalent FORMATETC structure
        [PreserveSig]
        Int32 GetCanonicalFormatEtc(
            ref ShellAPI.FORMATETC pformatetc,
            ref ShellAPI.FORMATETC pformatetcout);

        // Provides the source data object with data described by a 
        // FORMATETC structure and an STGMEDIUM structure
        [PreserveSig]
        Int32 SetData(
            ref ShellAPI.FORMATETC pformatetcIn,
            ref ShellAPI.STGMEDIUM pmedium, 
            bool frelease);

        // Creates and returns a pointer to an object to enumerate the 
        // FORMATETC supported by the data object
        [PreserveSig]
        Int32 EnumFormatEtc(
            int dwDirection,
            ref IEnumFORMATETC ppenumFormatEtc);

        // Creates a connection between a data object and an advise sink so 
        // the advise sink can receive notifications of changes in the data object
        [PreserveSig]
        Int32 DAdvise(
            ref ShellAPI.FORMATETC pformatetc,
            ref ShellAPI.ADVF advf,
            ref IAdviseSink pAdvSink, 
            ref int pdwConnection);

        // Destroys a notification previously set up with the DAdvise method
        [PreserveSig]
        Int32 DUnadvise(
            int dwConnection);
        
        // Creates and returns a pointer to an object to enumerate the current advisory connections
        [PreserveSig]
        Int32 EnumDAdvise(
            ref object ppenumAdvise);
    }
}

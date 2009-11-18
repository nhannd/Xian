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

namespace ClearCanvas.Controls.WinForms
{
	partial class Native
	{
		[ComImport]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("000214F2-0000-0000-C000-000000000046")]
		public interface IEnumIDList
		{
			// Retrieves the specified number of item identifiers in the enumeration sequence and advances the current position by the number of items retrieved. 
			[PreserveSig()]
			uint Next(
				uint celt, // Number of elements in the array pointed to by the rgelt parameter.
				out IntPtr rgelt, // Address of an array of ITEMIDLIST pointers that receives the item identifiers. The implementation must allocate these item identifiers using the Shell's allocator (retrieved by the SHGetMalloc function). 
				// The calling application is responsible for freeing the item identifiers using the Shell's allocator.
				out Int32 pceltFetched // Address of a value that receives a count of the item identifiers actually returned in rgelt. The count can be smaller than the value specified in the celt parameter. This parameter can be NULL only if celt is one. 
				);

			// Skips over the specified number of elements in the enumeration sequence. 
			[PreserveSig()]
			uint Skip(
				uint celt // Number of item identifiers to skip.
				);

			// Returns to the beginning of the enumeration sequence. 
			[PreserveSig()]
			uint Reset();

			// Creates a new item enumeration object with the same contents and state as the current one. 
			[PreserveSig()]
			uint Clone(
				out IEnumIDList ppenum // Address of a pointer to the new enumeration object. The calling application must eventually free the new object by calling its Release member function. 
				);
		}
	}
}
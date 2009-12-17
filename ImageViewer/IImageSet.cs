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
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IDisplaySet"/> objects.
	/// </summary>
	public interface IImageSet : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IImageSet"/> is not part of the 
		/// logical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="ILogicalWorkspace"/>
		/// </summary>
		/// <value>The parent <see cref="ILogicalWorkspace"/> or <b>null</b> if the 
		/// <see cref="IImageSet"/> has not been added to an 
		/// <see cref="ILogicalWorkspace"/> yet.</value>
		ILogicalWorkspace ParentLogicalWorkspace { get; }

		/// <summary>
		/// Gets the collection of <see cref="IDisplaySet"/> objects that belong
		/// to this <see cref="IImageSet"/>.
		/// </summary>
		DisplaySetCollection DisplaySets { get; }

		/// <summary>
		/// Gets a collection of linked <see cref="IDisplaySet"/> objects.
		/// </summary>
		IEnumerable<IDisplaySet> LinkedDisplaySets { get; }

		/// <summary>
		/// Gets the <see cref="IImageSetDescriptor"/> describing this <see cref="IImageSet"/>.
		/// </summary>
		IImageSetDescriptor Descriptor { get; }

		/// <summary>
		/// Gets the name of the image set.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the patient information associated with the image set.
		/// </summary>
		string PatientInfo { get; }

		/// <summary>
		/// Gets the unique identifier for this <see cref="IImageSet"/>.
		/// </summary>
		string Uid { get; }
	}
}

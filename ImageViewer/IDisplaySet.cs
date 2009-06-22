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
using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IPresentationImage"/> objects.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>Disposal</b> of an <see cref="IDisplaySet"/> and it's contained <see cref="IPresentationImage"/>s
	/// is handled by the framework when the <see cref="IDisplaySet"/> is part of the <see cref="IImageViewer"/>
	/// hierarchy.  When an <see cref="IImageViewer"/> is disposed, all its contained objects are disposed
	/// automatically.
	/// </para>
	/// <para>
	/// You may create copies of <see cref="IDisplaySet"/>s and <see cref="IPresentationImage"/>s
	/// (via the <see cref="CreateFreshCopy"/> mechanism) without keeping track of the objects and
	/// disposing them, provided the following two conditions are met:
	///  <list type="bullet">
	/// <item>
	/// The 'source' <see cref="IDisplaySet"/> (e.g. the one being copied) must be owned by an 
	/// <see cref="IImageSet"/> at the time the copy is created; the owner <see cref="IImageSet"/> must
	/// at some point become (or already be) part of the <see cref="IImageViewer"/> hierarchy.
	/// </item>
	/// <item>
	/// Any <see cref="IPresentationImage"/>s that are copied directly must be added to an <see cref="IDisplaySet"/> 
	/// that at some point becomes (or is already) part of the <see cref="IImageViewer"/> hierarchy.
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>
	public interface IDisplaySet : IDrawable, IMemorable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IDisplaySet"/> is not part of the 
		/// logical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IImageSet"/>.
		/// </summary>
		/// <value>The parent <see cref="ImageSet"/> or <b>null</b> if the 
		/// <see cref="IDisplaySet"/> has not been added to an 
		/// <see cref="IImageSet"/> yet.</value>
		IImageSet ParentImageSet { get; }

		/// <summary>
		/// Gets the collection of <see cref="IPresentationImage"/> objects belonging
		/// to this <see cref="IDisplaySet"/>.
		/// </summary>
		PresentationImageCollection PresentationImages { get; }

		/// <summary>
		/// Gets a collection of linked <see cref="IPresentationImage"/> objects.
		/// </summary>
		IEnumerable<IPresentationImage> LinkedPresentationImages { get; }

		/// <summary>
		/// Gets the <see cref="IImageBox"/> associated with this <see cref="IDisplaySet"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageBox "/> or <b>null</b> if the
		/// <see cref="IDisplaySet"/> is not currently visible.</value>
		IImageBox ImageBox { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IImageBox"/> is
		/// linked.
		/// </summary>
		/// <value><b>true</b> if linked; <b>false</b> otherwise.</value>
		/// <remarks>
		/// Multiple display sets may be linked, allowing tools that can operate on
		/// multiple display sets to operate on all linked display sets simultaneously.  
		/// Note that the concept of linkage is slightly different from selection:
		/// it is possible for an <see cref="IDisplaySet"/> to be 1) selected but not linked
		/// 2) linked but not selected and 3) selected and linked.
		/// </remarks>
		bool Linked { get; set; }

		/// <summary>
		/// Gets or sets the name of the display set.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a text description for the display set.
		/// </summary>
		/// <remarks>
		/// This value will normally correspond to the series description of the contained images.
		/// </remarks>
		string Description { get; }

		/// <summary>
		/// Gets the numeric identifier for the display set.
		/// </summary>
		/// <remarks>
		/// This value will normally correspond to the series number of the contained DICOM images.
		/// </remarks>
		int Number { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="IDisplaySet"/> is selected.
		/// </summary>
		bool Selected { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="IDisplaySet"/> is visible.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Gets a unique identifier for this <see cref="IDisplaySet"/>.
		/// </summary>
		string Uid { get; }

		/// <summary>
		/// Creates a fresh copy of the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="IDisplaySet"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		IDisplaySet CreateFreshCopy();

		/// <summary>
		/// Creates a deep copy of the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IDisplaySet"/>s may not return null from this method.
		/// </remarks>
		IDisplaySet Clone();
	}
}

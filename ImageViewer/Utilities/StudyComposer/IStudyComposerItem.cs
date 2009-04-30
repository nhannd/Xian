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

using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// An interface for items in the <see cref="StudyComposerComponent"/> tree.
	/// </summary>
	public interface IStudyComposerItem
	{
		/// <summary>
		/// Indicates that a property on the node has changed, and that any views should refresh its display of the item.
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Gets or sets the name label of this item.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets a short, multi-line description of the item that contains ancillary information.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> node that is encapsulated by this <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		StudyBuilderNode Node { get; }

		/// <summary>
		/// Gets an <see cref="Image"/> icon that can be used to represent the item in thumbnail views.
		/// </summary>
		Image Icon { get; }

		/// <summary>
		/// Regenerates the icon for a specific icon size.
		/// </summary>
		/// <param name="iconSize">The <see cref="Size"/> of the icon to generate.</param>
		void UpdateIcon(Size iconSize);

		/// <summary>
		/// Regenerates the icon for the default icon size (64x64).
		/// </summary>
		void UpdateIcon();

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		IStudyComposerItem Clone();
	}
}
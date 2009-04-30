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

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Provides text to be rendered to the overlay by an <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/>.
	/// </summary>
	/// <seealso cref="AnnotationBox"/>
	/// <seealso cref="AnnotationItemConfigurationOptions"/>
	/// <seealso cref="IAnnotationItemProvider"/>
	/// <seealso cref="IAnnotationLayout"/>
	/// <seealso cref="IAnnotationLayoutProvider"/>
	public interface IAnnotationItem
	{

		/// <summary>
		/// Gets a unique identifier.
		/// </summary>
		string GetIdentifier();

		/// <summary>
		/// Gets a user friendly display name.
		/// </summary>
		string GetDisplayName();

		/// <summary>
		/// Gets the label that can be shown on the overlay depending on the <see cref="AnnotationBox"/>'s 
		/// configuration (<see cref="AnnotationItemConfigurationOptions"/>).
		/// </summary>
		string GetLabel();

		/// <summary>
		/// Gets the annotation text to display on the overlay for <paramref name="presentationImage"/>.
		/// </summary>
		string GetAnnotationText(IPresentationImage presentationImage);
	}
}

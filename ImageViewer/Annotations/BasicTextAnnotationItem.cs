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

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An <see cref="AnnotationItem"/> that returns fixed text from <see cref="GetAnnotationText"/>.
	/// </summary>
	/// <seealso cref="AnnotationItem"/>
	public class BasicTextAnnotationItem : AnnotationItem
	{
		private string _annotationText;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="BasicTextAnnotationItem"/>.</param>
		/// <param name="displayName">The <see cref="BasicTextAnnotationItem"/>'s display name.</param>
		/// <param name="label">The <see cref="BasicTextAnnotationItem"/>'s label.</param>
		/// <param name="annotationText">The text to return from <see cref="GetAnnotationText"/>.</param>
		public BasicTextAnnotationItem(string identifier, string displayName, string label, string annotationText)
			: base(identifier, displayName, label)
		{
			Platform.CheckForEmptyString(annotationText, "annotationText");
			_annotationText = annotationText;
		}

		/// <summary>
		/// Gets or sets the text to be returned from <see cref="GetAnnotationText"/>.
		/// </summary>
		public string AnnotationText
		{
			get { return _annotationText; }
			set
			{
				Platform.CheckForEmptyString(value, "value");
				_annotationText = value;
			}
		}

		/// <summary>
		/// Gets the annotation text for display on the overlay.
		/// </summary>
		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			return _annotationText;
		}
	}
}

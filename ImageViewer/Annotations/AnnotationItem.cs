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
	/// Base implementation of <see cref="IAnnotationItem"/>.
	/// </summary>
	/// <seealso cref="IAnnotationItem"/>
	public abstract class AnnotationItem : IAnnotationItem
	{
		private readonly string _identifier;
		private readonly string _displayName;
		private readonly string _label;

		/// <summary>
		/// A constructor that uses the <see cref="AnnotationItem"/>'s unique identifier to determine
		/// the display name and label using an <see cref="IAnnotationResourceResolver"/>.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItem"/>.</param>
		/// <param name="resolver">The object that will resolve the display name and label 
		/// from the <see cref="AnnotationItem"/>'s unique identifier.</param>
		protected AnnotationItem(string identifier, IAnnotationResourceResolver resolver)
			: this(identifier, resolver.ResolveDisplayName(identifier), resolver.ResolveLabel(identifier))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItem"/>.</param>
		/// <param name="displayName">The <see cref="AnnotationItem"/>'s display name.</param>
		/// <param name="label">The <see cref="AnnotationItem"/>'s label.</param>
		protected AnnotationItem(string identifier, string displayName, string label)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForEmptyString(displayName, "displayName");

			_identifier = identifier;
			_displayName = displayName;
			_label = label ?? "";
		}

		#region IAnnotationItem Members

		/// <summary>
		/// Gets a unique identifier.
		/// </summary>
		public string GetIdentifier()
		{
			return _identifier;
		}

		/// <summary>
		/// Gets a user friendly display name.
		/// </summary>
		public string GetDisplayName()
		{
			return _displayName;
		}

		/// <summary>
		/// Gets the label that can be shown on the overlay depending on the <see cref="AnnotationBox"/>'s 
		/// configuration (see <see cref="AnnotationItemConfigurationOptions"/>).
		/// </summary>
		public string GetLabel()
		{
			return _label;
		}

		/// <summary>
		/// Gets the annotation text to display on the overlay for <paramref name="presentationImage"/>.
		/// </summary>
		public abstract string GetAnnotationText(IPresentationImage presentationImage);

		#endregion
	}
}

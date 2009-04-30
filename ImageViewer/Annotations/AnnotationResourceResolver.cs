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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Base implementation of <see cref="IAnnotationResourceResolver"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Uses assembly resources to look up the display name and label 
	/// for an <see cref="IAnnotationItem"/> (and <see cref="IAnnotationItemProvider"/>) based on it's 
	/// unique identifier.
	/// </para>
	/// <para>
	/// The algorithm used is quite simple; all '.' characters in the unique identifier are replaced with '_' because
	/// the resource editor doesn't like '.'s, and one of the keywords "_DisplayName" or "_Label" is appended, giving
	/// the resource identifier to lookup.
	/// </para>
	/// <para>
	/// An example would be a unique identifier of "Dicom.GeneralSeries.SeriesDescription".  The resource identifiers
	/// to lookup would be "Dicom_GeneralSeries_SeriesDescription_DisplayName" and "Dicom_GeneralSeries_SeriesDescription_Label", 
	/// respectively.
	/// </para>
	/// </remarks>
	/// <seealso cref="IAnnotationResourceResolver"/>
	/// <seealso cref="ResourceResolver"/>
	public sealed class AnnotationResourceResolver : ResourceResolver, IAnnotationResourceResolver
	{
		private readonly char replaceChar = '.';
		private readonly char replaceWithChar = '_';

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="target">The target object from which to determine the <see cref="Assembly"/> 
		/// whose resources are to be used to lookup the display name and label.</param>
		public AnnotationResourceResolver(object target)
			: base(new Assembly[] { target.GetType().Assembly })
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="assembly">The <see cref="Assembly"/> whose resources 
		/// are to be used to lookup the display name and label.</param>
		public AnnotationResourceResolver(Assembly assembly)
			: base(assembly)
		{
		}

		/// <summary>
		/// Resolves the <see cref="IAnnotationItem"/>'s label (see <see cref="IAnnotationItem.GetLabel()"/>).
		/// </summary>
		public string ResolveLabel(string annotationIdentifier)
		{
			Platform.CheckForEmptyString(annotationIdentifier, "annotationIdentifier"); 
			
			string resourceString = String.Format("{0}{1}{2}", annotationIdentifier, replaceChar, "Label");
			resourceString = resourceString.Replace(replaceChar, replaceWithChar);

			return base.LocalizeString(resourceString) ?? "";
		}

		/// <summary>
		/// Resolves the <see cref="IAnnotationItem"/>'s (or <see cref="IAnnotationItemProvider"/>'s) display name 
		/// (see <see cref="IAnnotationItem.GetDisplayName"/> and <see cref="IAnnotationItemProvider.GetDisplayName"/>).
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when the display name cannot be resolved.</exception>
		public string ResolveDisplayName(string annotationIdentifier)
		{
			Platform.CheckForEmptyString(annotationIdentifier, "annotationIdentifier"); 
			
			string resourceString = String.Format("{0}{1}{2}", annotationIdentifier, replaceChar, "DisplayName");
			resourceString = resourceString.Replace(replaceChar, replaceWithChar);

			string displayName = base.LocalizeString(resourceString);

			if (displayName == resourceString)
				throw new InvalidOperationException(String.Format(SR.ExceptionFormatAnnotationItemHasNoDisplayName, annotationIdentifier));

			return displayName;
		}
	}
}

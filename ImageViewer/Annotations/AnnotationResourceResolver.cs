#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

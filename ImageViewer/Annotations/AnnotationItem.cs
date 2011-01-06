#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

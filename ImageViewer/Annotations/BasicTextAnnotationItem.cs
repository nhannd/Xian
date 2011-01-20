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

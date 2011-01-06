#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Configures how the <see cref="IAnnotationItem"/> text should be formatted/shown 
	/// in an <see cref="AnnotationBox"/>.
	/// </summary>
	/// <seealso cref="IAnnotationItem"/>
	[Cloneable(true)]
	public sealed class AnnotationItemConfigurationOptions
	{
		private bool _showLabel = false;
		private bool _showLabelIfValueEmpty = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		public AnnotationItemConfigurationOptions()
		{
		}

		/// <summary>
		/// Gets or sets whether or not to show the label (<see cref="IAnnotationItem.GetLabel"/>).
		/// </summary>
		public bool ShowLabel
		{
			get { return _showLabel; }
			set { _showLabel = value; }
		}

		/// <summary>
		/// Gets or sets whether or not to show the label (<see cref="IAnnotationItem.GetLabel"/>) even
		/// if the overlay text (<see cref="IAnnotationItem.GetAnnotationText"/>) is empty.
		/// </summary>
		public bool ShowLabelIfValueEmpty
		{
			get { return _showLabelIfValueEmpty; }
			set { _showLabelIfValueEmpty = value; }
		}

		/// <summary>
		/// Creates a deep clone of this object.
		/// </summary>
		public AnnotationItemConfigurationOptions Clone()
		{
			AnnotationItemConfigurationOptions clone = new AnnotationItemConfigurationOptions();
			clone._showLabel = this._showLabel;
			clone._showLabelIfValueEmpty = this._showLabelIfValueEmpty;
			return clone;
		}
	}
}

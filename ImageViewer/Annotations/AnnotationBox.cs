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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An <see cref="AnnotationBox"/> is rendered to the screen by an <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/>.
	/// </summary>
	/// <seealso cref="AnnotationItemConfigurationOptions"/>
	/// <seealso cref="IAnnotationItem"/>
	[Cloneable(true)]
	public sealed class AnnotationBox
	{
		/// <summary>
		/// Defines the available truncation behaviours for strings that will extend beyond <see cref="AnnotationBox.NormalizedRectangle"/>.
		/// </summary>
		public enum TruncationBehaviour
		{
			/// <summary>
			/// Specifies that the string should just be cut off at the edge of the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Truncate, 
			
			/// <summary>
			/// Specifies that the string should have an ellipses (...) at the edge of the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Ellipsis
		};

		/// <summary>
		/// Defines the available horizontal justifications.
		/// </summary>
		public enum JustificationBehaviour
		{
			/// <summary>
			/// Specifies that the string should be left-justified in the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Left, 

			/// <summary>
			/// Specifies that the string should be centred horizontally in the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Center, 
			
			/// <summary>
			/// Specifies that the string should be right-justified in the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Right
		};

		/// <summary>
		/// Defines the available vertical alignments.
		/// </summary>
		public enum VerticalAlignmentBehaviour
		{
			/// <summary>
			/// Specifies that the string should be aligned along the top of the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Top,

			/// <summary>
			/// Specifies that the string should be centered in the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Center,

			/// <summary>
			/// Specifies that the string should be aligned along the bottom of the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Bottom
		};

		#region Private Fields

		[CloneCopyReference]
		private IAnnotationItem _annotationItem;
		private AnnotationItemConfigurationOptions _annotationItemConfigurationOptions;

		private RectangleF _normalizedRectangle;
		
		private byte _numberOfLines = 1;

		private static readonly string _defaultFont = "Arial";
		private static readonly string _defaultColor = "WhiteSmoke";

		private string _font = _defaultFont;
		private string _color = _defaultColor; 
		
		private bool _bold = false;
		private bool _italics = false;
		private bool _fitWidth = false;
		private bool _alwaysVisible = false;

		private TruncationBehaviour _truncation = TruncationBehaviour.Ellipsis;
		private JustificationBehaviour _justification = JustificationBehaviour.Left;
		private VerticalAlignmentBehaviour _verticalAlignment = VerticalAlignmentBehaviour.Center;

		private bool _visible = true;

		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AnnotationBox()
		{
			this.NormalizedRectangle = new RectangleF();
		}

		/// <summary>
		/// Constructor that initializes the <see cref="NormalizedRectangle"/> and <see cref="AnnotationItem"/> properties.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when the input <paramref name="normalizedRectangle"/> is not normalized.</exception>
		public AnnotationBox(RectangleF normalizedRectangle, IAnnotationItem annotationItem)
		{
			RectangleUtilities.VerifyNormalizedRectangle(normalizedRectangle); 
			this.NormalizedRectangle = normalizedRectangle;
			_annotationItem = annotationItem;
		}

		/// <summary>
		/// Gets the text to be rendered into the area defined by <see cref="NormalizedRectangle"/> for the input <paramref name="presentationImage"/>.
		/// </summary>
		/// <param name="presentationImage">The presentation image.</param>
		public string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (_annotationItem == null)
				return string.Empty;

			string annotationText = _annotationItem.GetAnnotationText(presentationImage);
			string annotationLabel = _annotationItem.GetLabel();

			if (string.IsNullOrEmpty(annotationText))
			{
				if (this.ConfigurationOptions.ShowLabelIfValueEmpty)
					annotationText = string.Format("{0}: -", annotationLabel);
			}
			else if (this.ConfigurationOptions.ShowLabel)
			{
				annotationText = string.Format("{0}: {1}", annotationLabel, annotationText);
			}

			return annotationText;
		}

		/// <summary>
		/// Gets the associated <see cref="IAnnotationItem"/> that provides the annotation text.
		/// </summary>
		/// <remarks>
		/// It is permissible for this value to be null.  A value of "" will always be returned from <see cref="GetAnnotationText"/>.
		/// </remarks>
		public IAnnotationItem AnnotationItem
		{
			get { return _annotationItem; }
			set { _annotationItem = value; }
		}

		/// <summary>
		/// Defines the normalized rectangle in which the <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/> should render the text.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when setting the property if the value is not normalized.</exception>
		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
			set 
			{
				RectangleUtilities.VerifyNormalizedRectangle(value);
				_normalizedRectangle = value;
			}
		}

		/// <summary>
		/// Defines configuration options for how <see cref="GetAnnotationText"/> should format its return value.
		/// </summary>
		public AnnotationItemConfigurationOptions ConfigurationOptions
		{
			get
			{
				if (_annotationItemConfigurationOptions == null)
					_annotationItemConfigurationOptions = new AnnotationItemConfigurationOptions();

				return _annotationItemConfigurationOptions;
			}
			set { _annotationItemConfigurationOptions = value; }
		}

		/// <summary>
		/// Gets the default font ("Arial") by name.
		/// </summary>
		public static string DefaultFont
		{
			get { return _defaultFont; }
		}

		/// <summary>
		/// Gets the default color ("WhiteSmoke") by name.
		/// </summary>
		public static string DefaultColor
		{
			get { return _defaultColor; }
		}

		/// <summary>
		/// Gets or sets the font (by name) that should be used to render the text.
		/// </summary>
		/// <remarks>
		/// The default value is "Arial".
		/// </remarks>
		public string Font
		{
			get { return _font; }
			set
			{
				Platform.CheckForEmptyString(value, "value");
				_font = value;
			}
		}

		/// <summary>
		/// Gets or sets the color (by name) that should be used to render the text.
		/// </summary>
		/// <remarks>
		/// The default value is "WhiteSmoke".
		/// </remarks>
		public string Color
		{
			get { return _color; }
			set
			{
				Platform.CheckForEmptyString(value, "value");
				_color = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the text should be in italics.
		/// </summary>
		/// <remarks>
		/// The default value is false.
		/// </remarks>
		public bool Italics
		{
			get { return _italics; }
			set { _italics = value; }
		}

		/// <summary>
		/// Gets or sets whether the text should be in bold.
		/// </summary>
		/// <remarks>
		/// The default value is false.
		/// </remarks>
		public bool Bold
		{
			get { return _bold; }
			set { _bold = value; }
		}

		/// <summary>
		/// Gets or sets the number of lines of to render.
		/// </summary>
		/// <remarks>
		/// The default value is 1.
		/// </remarks>
		public byte NumberOfLines
		{
			get { return _numberOfLines; }
			set
			{
				//you cannot have multiple lines in the 'fit width' scenario.
				if (value > 1 && _fitWidth)
					return;
					
				_numberOfLines = Math.Max((byte)1, value);
			}
		}

		/// <summary>
		/// Gets or sets whether the text should be fit to the width of the <see cref="NormalizedRectangle"/>.
		/// </summary>
		/// <remarks>
		/// The default value is false.
		/// </remarks>
		public bool FitWidth
		{
			get { return _fitWidth; }
			set
			{
				_fitWidth = value;
				
				//you cannot have multiple lines in the 'fit width' scenario.
				if (_numberOfLines > 1)
					_numberOfLines = 1;
			}
		}

		/// <summary>
		/// Gets or sets whether or not the item can be made invisible.
		/// </summary>
		public bool AlwaysVisible
		{
			get { return _alwaysVisible; }
			set { _alwaysVisible = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="TruncationBehaviour"/>.
		/// </summary>
		/// <remarks>
		/// The default value is <see cref="TruncationBehaviour.Ellipsis"/>.
		/// </remarks>
		public TruncationBehaviour Truncation
		{
			get { return _truncation; }
			set { _truncation = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="JustificationBehaviour"/>.
		/// </summary>
		/// <remarks>
		/// The default value is <see cref="JustificationBehaviour.Left"/>.
		/// </remarks>
		public JustificationBehaviour Justification
		{
			get { return _justification; }
			set { _justification = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="VerticalAlignmentBehaviour"/>.
		/// </summary>
		/// <remarks>
		/// The default value is <see cref="VerticalAlignmentBehaviour.Center"/>.
		/// </remarks>
		public VerticalAlignmentBehaviour VerticalAlignment
		{
			get { return _verticalAlignment; }
			set { _verticalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets whether or not the item is visible.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_alwaysVisible)
					value = true;

				_visible = value;
			}
		}

		/// <summary>
		/// Creates a deep clone of this object.
		/// </summary>
		public AnnotationBox Clone()
		{
			return CloneBuilder.Clone(this) as AnnotationBox;
		}
	}
}

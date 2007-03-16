using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Annotations
{
	public sealed class AnnotationBox
	{
		public enum TruncationBehaviour { Truncate, Ellipsis };
		public enum JustificationBehaviour { Near, Center, Far };
		public enum VerticalAlignmentBehaviour { Top, Center, Bottom };

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

		private TruncationBehaviour _truncation = TruncationBehaviour.Ellipsis;
		private JustificationBehaviour _justification = JustificationBehaviour.Near;
		private VerticalAlignmentBehaviour _verticalAlignment = VerticalAlignmentBehaviour.Center;

		public AnnotationBox()
		{
			this.NormalizedRectangle = new RectangleF();
		}

		public AnnotationBox(RectangleF normalizedRectangle, IAnnotationItem annotationItem)
		{
			this.NormalizedRectangle = normalizedRectangle;
			_annotationItem = annotationItem;
		}

		public string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (_annotationItem == null)
				return string.Empty;

			string annotationText = _annotationItem.GetAnnotationText(presentationImage);
			string annotationLabel = _annotationItem.GetLabel();

			if (string.IsNullOrEmpty(annotationText))
			{
				if (this.ConfigurationOptions.ShowLabelIfValueEmpty)
					annotationText = string.Format("{0}:", annotationLabel);
			}
			else if (this.ConfigurationOptions.ShowLabel)
			{
				annotationText = string.Format("{0}: {1}", annotationLabel, annotationText);
			}

			return annotationText;
		}

		public IAnnotationItem AnnotationItem
		{
			get { return _annotationItem; }
			set { _annotationItem = value; }
		}

		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
			set 
			{
				RectangleUtilities.VerifyNormalizedRectangle(value);
				_normalizedRectangle = value;
			}
		}

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

		public static string DefaultFont
		{
			get { return _defaultFont; }
		}

		public static string DefaultColor
		{
			get { return _defaultColor; }
		}

		public string Font
		{
			get { return _font; }
			set
			{
				Platform.CheckForEmptyString(value, "value");
				_font = value;
			}
		}

		public string Color
		{
			get { return _color; }
			set
			{
				Platform.CheckForEmptyString(value, "value");
				_color = value;
			}
		}

		public bool Italics
		{
			get { return _italics; }
			set { _italics = value; }
		}
	
		public bool Bold
		{
			get { return _bold; }
			set { _bold = value; }
		}

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

		public TruncationBehaviour Truncation
		{
			get { return _truncation; }
			set { _truncation = value; }
		}

		public JustificationBehaviour Justification
		{
			get { return _justification; }
			set { _justification = value; }
		}

		public VerticalAlignmentBehaviour VerticalAlignment
		{
			get { return _verticalAlignment; }
			set { _verticalAlignment = value; }
		}

		public AnnotationBox Clone()
		{
			AnnotationBox newBox = new AnnotationBox(this.NormalizedRectangle, this.AnnotationItem);
			if (this.ConfigurationOptions != null)
				newBox.ConfigurationOptions = this.ConfigurationOptions.Clone();

			newBox.Font = this.Font;
			newBox.Color = this.Color;
			newBox.Italics = this.Italics;
			newBox.Bold = this.Bold;
			newBox.NumberOfLines = this.NumberOfLines;
			newBox.Truncation = this.Truncation;
			newBox.Justification = this.Justification;
			newBox.VerticalAlignment = this.VerticalAlignment;
			newBox.FitWidth = this.FitWidth;

			return newBox;
		}
	}
}

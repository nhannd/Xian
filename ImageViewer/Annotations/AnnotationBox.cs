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
		public enum TruncationBehaviour { TRUNCATE, ELLIPSES };
		public enum JustificationBehaviour { NEAR, CENTRE, FAR };

		private IAnnotationItem _annotationItem;
		private AnnotationItemConfigurationOptions _annotationItemConfigurationOptions;

		private RectangleF _normalizedRectangle;
		
		private byte _numberOfLines = 1;

		private static readonly string _defaultFont = "Arial";
		private static readonly string _defaultColor = "White";

		private string _font = _defaultFont;
		private string _color = _defaultColor; 
		
		private bool _bold = false;
		private bool _italics = false;
		
		private TruncationBehaviour _truncation = TruncationBehaviour.ELLIPSES;
		private JustificationBehaviour _justification = JustificationBehaviour.NEAR;

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
				if (FloatComparer.IsLessThan(value.Left, 0.0f) ||
					FloatComparer.IsGreaterThan(value.Left, 1.0f) ||
					FloatComparer.IsLessThan(value.Right, 0.0f) ||
					FloatComparer.IsGreaterThan(value.Right, 1.0f) ||
					FloatComparer.IsLessThan(value.Top, 0.0f) ||
					FloatComparer.IsGreaterThan(value.Top, 1.0f) ||
					FloatComparer.IsLessThan(value.Bottom, 0.0f) ||
					FloatComparer.IsGreaterThan(value.Bottom, 1.0f) ||
					FloatComparer.IsGreaterThan(value.Left, value.Right) ||
					FloatComparer.IsGreaterThan(value.Top, value.Bottom))
				{
					throw new ArgumentException(String.Format(SR.ExceptionInvalidNormalizedRectangle, value.Top.ToString(), value.Left.ToString(), value.Bottom.ToString(), value.Right.ToString()));
				}

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
			set { _numberOfLines = Math.Max((byte)1, value); }
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

			return newBox;
		}
	}
}

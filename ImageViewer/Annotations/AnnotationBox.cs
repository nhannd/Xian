using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Annotations
{
	public class AnnotationBox
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
		
		public AnnotationBox(RectangleF normalizedRectangle)
		{
			_normalizedRectangle = normalizedRectangle;
		}

		public string GetAnnotationText(PresentationImage presentationImage)
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
			set { _normalizedRectangle = value; }
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
			set { _font = value; }
		}

		public string Color
		{
			get { return _color; }
			set { _color = value; }
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
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	public class AnnotationItemConfigurationOptions
	{
		private bool _showLabel = false;
		private bool _showLabelIfValueEmpty = false;

		public bool ShowLabel
		{
			get { return _showLabel; }
			set { _showLabel = value; }
		}

		public bool ShowLabelIfValueEmpty
		{
			get { return _showLabelIfValueEmpty; }
			set { _showLabelIfValueEmpty = value; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	public sealed class AnnotationItemConfigurationOptions
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

		public AnnotationItemConfigurationOptions Clone()
		{
			AnnotationItemConfigurationOptions newItem = new AnnotationItemConfigurationOptions();

			newItem.ShowLabel = this.ShowLabel;
			newItem.ShowLabelIfValueEmpty = this.ShowLabelIfValueEmpty;

			return newItem;
		}
	}
}

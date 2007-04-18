using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	internal sealed class VoiLutPreset
	{
		private string _name;
		private string _modalityFilter;
		private XKeys _keyStroke;
		private IVoiLutPresetApplicator _lutPresetApplicator;

		public VoiLutPreset(string name, string modalityFilter, XKeys keyStroke, IVoiLutPresetApplicator lutPresetApplicator)
		{ 
			Platform.CheckForEmptyString(name, "name");
			Platform.CheckForNullReference(lutPresetApplicator, "lutPresetApplicator");

			_name = name;
			_modalityFilter = modalityFilter;
			_keyStroke = keyStroke;
			_lutPresetApplicator = lutPresetApplicator;
		}

		#region IVoiLutPreset Members

		public string Name
		{
			get { return _name; }
		}

		public bool AppliesTo(IPresentationImage image)
		{
			return _lutPresetApplicator.AppliesTo(image) && this.IsModalityMatch(image);
		}

		public void Apply(IPresentationImage image)
		{
			if (!AppliesTo(image))
				throw new InvalidOperationException(SR.ExceptionLutPresetIsNotApplicableForTheProvidedImage);

			_lutPresetApplicator.Apply(image);
		}

		#endregion

		private bool IsModalityMatch(IPresentationImage image)
		{
			if (String.IsNullOrEmpty(_modalityFilter))
				return true;

			if (!(image is IImageSopProvider))
				return false;

			return ((image as IImageSopProvider).ImageSop.Modality == _modalityFilter);
		}
	}
}

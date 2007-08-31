using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[ExtensionOf(typeof(PresetVoiLutApplicatorFactoryExtensionPoint))]
	public sealed class LinearPresetVoiLutApplicatorFactory : PresetVoiLutApplicatorFactory<LinearPresetVoiLutApplicator>
	{
		public LinearPresetVoiLutApplicatorFactory()
		{
		}

		public override string Name
		{
			get { return "Linear Preset"; }
		}
	}

	public sealed class LinearPresetVoiLutApplicator : PresetVoiLutApplicator
	{
		private string _presetName;
		private double _windowWidth;
		private double _windowCenter;

		public LinearPresetVoiLutApplicator()
		{
			_presetName = null;
			_windowWidth = double.NaN;
			_windowCenter = double.NaN;
		}

		public override string Name
		{
			get { return _presetName; }
		}

		public override string Description
		{
			get { return String.Format("{0}: WW = {1}, WC = {2}", _presetName, _windowWidth, _windowCenter); }
		}

		[SimpleSerialized]
		public string PresetName
		{
			get { return _presetName; }
			set { _presetName = value; }
		}

		[SimpleSerialized]
		public double WindowCenter
		{
			get { return _windowCenter; }
			set { _windowCenter = value; }
		}

		[SimpleSerialized]
		public double WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		private PresetVoiLutLinear.PresetVoiLutLinearParameters Parameters
		{
			get
			{
				return new PresetVoiLutLinear.PresetVoiLutLinearParameters(this.PresetName, this.WindowWidth, this.WindowCenter);
			}
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
			return (presentationImage is IVoiLutProvider);
		}

		public override void Apply(IPresentationImage image)
		{
			if (!AppliesTo(image))
				throw new ArgumentException(String.Format("The input presentation image must implement {0}", typeof (IVoiLutProvider).Name));

			IVoiLutManager manager = ((IVoiLutProvider)image).VoiLutManager;

			PresetVoiLutLinear currentLut = manager.GetLut() as PresetVoiLutLinear;
			if (currentLut == null)
				manager.InstallLut(new PresetVoiLutLinear(this.Parameters));
			else
				currentLut.Parameters = this.Parameters;
		}

		public override void Validate()
		{
			if (double.IsNaN(_windowWidth) || _windowWidth < 1)
				throw new InvalidOperationException(String.Format("The value '{0}' is invalid for Window Width", _windowWidth));
			if (double.IsNaN(_windowCenter))
				throw new InvalidOperationException(String.Format("The value '{0}' is invalid for Window Center", _windowCenter));
			if (String.IsNullOrEmpty(_presetName))
				throw new InvalidOperationException("The Preset Name cannot be empty");
		}
	}
}

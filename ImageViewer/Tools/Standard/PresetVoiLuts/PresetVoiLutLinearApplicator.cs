using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[ExtensionOf(typeof(PresetVoiLutApplicatorFactoryExtensionPoint))]
	public sealed class PresetVoiLutLinearApplicatorFactory : PresetVoiLutApplicatorFactory<PresetVoiLutLinearApplicator, EditPresetVoiLutLinearComponent>
	{
		internal static readonly string FactoryName = "Linear Preset";

		public PresetVoiLutLinearApplicatorFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}
	}

	public sealed class PresetVoiLutLinearApplicator : PresetVoiLutApplicator
	{
		private string _presetName;
		private double _windowWidth;
		private double _windowCenter;

		public PresetVoiLutLinearApplicator()
		{
			_presetName = "";
			_windowWidth = 1;
			_windowCenter = 0;
		}

		public override string Name
		{
			get { return _presetName; }
		}

		public override string Description
		{
			get { return String.Format("W = {0}, L = {1}", _windowWidth, _windowCenter); }
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

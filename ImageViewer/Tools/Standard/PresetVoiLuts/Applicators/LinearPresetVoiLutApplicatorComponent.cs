using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	[ExtensionPoint]
	public sealed class LinearPresetVoiLutApplicatorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AllowMultiplePresetVoiLutApplicators]
	[AssociateView(typeof(LinearPresetVoiLutApplicatorComponentViewExtensionPoint))]
	public sealed class LinearPresetVoiLutApplicatorComponent : PresetVoiLutApplicatorComponent
	{
		private string _name;
		private double _windowWidth;
		private double _windowCenter;

		public LinearPresetVoiLutApplicatorComponent()
		{
			_name = "";
			_windowWidth = double.NaN;
			_windowCenter = double.NaN;
		}

		public override string Name
		{
			get { return _name; }
		}

		public override string Description
		{
			get { return String.Format("W = {0}, L = {1}", this.WindowWidth, this.WindowCenter); }
		}
		
		[SimpleSerialized]
		public string PresetName
		{
			get { return _name; }
			set
			{
				if (_name == value)
					return;
				
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		[SimpleSerialized]
		public double WindowWidth
		{
			get { return _windowWidth; }
			set
			{
				if (_windowWidth == value)
					return;

				_windowWidth = value;
				OnPropertyChanged("WindowWidth");
			}
		}

		[SimpleSerialized]
		public double WindowCenter
		{
			get { return _windowCenter; }
			set
			{
				if (_windowCenter == value)
					return;

				_windowCenter = value;
				OnPropertyChanged("WindowCenter");
			}
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
			return (presentationImage is IVoiLutProvider);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new ArgumentException(String.Format("The input presentation image must implement {0}", typeof(IVoiLutProvider).Name));

			IVoiLutManager manager = ((IVoiLutProvider)presentationImage).VoiLutManager;

			PresetVoiLutLinear.PresetVoiLutLinearParameters parameters = new PresetVoiLutLinear.PresetVoiLutLinearParameters(this.Name, this.WindowWidth, this.WindowCenter);

			PresetVoiLutLinear currentLut = manager.GetLut() as PresetVoiLutLinear;
			if (currentLut == null)
				manager.InstallLut(new PresetVoiLutLinear(parameters));
			else
				currentLut.Parameters = parameters;
		}

		public override void Start()
		{
			if (this.WindowWidth < 1 || double.IsNaN(this.WindowWidth))
				this.WindowWidth = 1;
			if (double.IsNaN(this.WindowCenter))
				this.WindowCenter = 0;

			UpdateValid();

			base.Modified = false;

			base.Start();
		}

		public override void Validate()
		{
			if (String.IsNullOrEmpty(this.Name))
				throw new InvalidOperationException("The Preset Name cannot be empty");
			if (double.IsNaN(this.WindowWidth) || this.WindowWidth < 1)
				throw new InvalidOperationException(String.Format("The value '{0}' is invalid for Window Width", this.WindowWidth));
			if (double.IsNaN(this.WindowCenter))
				throw new InvalidOperationException(String.Format("The value '{0}' is invalid for Window Center", this.WindowCenter));
		}

		protected override void UpdateValid()
		{
			bool valid = true;
			if (String.IsNullOrEmpty(this.Name))
				valid = false;

			if (this.WindowWidth < 1 || double.IsNaN(this.WindowWidth))
				valid = false;

			if (double.IsNaN(this.WindowCenter))
				valid = false;

			base.Valid = valid;
		}
	}
}
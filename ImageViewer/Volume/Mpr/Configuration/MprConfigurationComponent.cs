using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Configuration
{
	[ExtensionPoint]
	public sealed class MprConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (MprConfigurationComponentViewExtensionPoint))]
	public class MprConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string Path = "Mpr";

		private MprSettings _settings;
		private float _sliceSpacingFactor = 1;
		private bool _autoSliceSpacing = true;

		[ValidateGreaterThan(0f, Inclusive = false, Message = "Slice spacing must be greater than zero and less than or equal to 5.")]
		[ValidateLessThan(5f, Inclusive = true, Message = "Slice spacing must be greater than zero and less than or equal to 5.")]
		public float SliceSpacingFactor
		{
			get { return _sliceSpacingFactor; }
			set
			{
				if (_sliceSpacingFactor != value)
				{
					_sliceSpacingFactor = value;
					this.Modified = true;
					this.NotifyPropertyChanged("SliceSpacingFactor");
				}
			}
		}

		public bool AutoSliceSpacing
		{
			get { return _autoSliceSpacing; }
			set
			{
				if (_autoSliceSpacing != value)
				{
					_autoSliceSpacing = value;
					this.Modified = true;
					this.NotifyPropertyChanged("AutoSliceSpacing");
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_settings = MprSettings.Default;
			_sliceSpacingFactor = _settings.SliceSpacingFactor;
			_autoSliceSpacing = _settings.AutoSliceSpacing;
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}

		public override void Save()
		{
			_settings.AutoSliceSpacing = _autoSliceSpacing;
			_settings.SliceSpacingFactor = _sliceSpacingFactor;
			_settings.Save();
		}
	}
}
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.Config
{
	[ExtensionPoint]
	public class StandardToolsConfigComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (StandardToolsConfigComponentViewExtensionPoint))]
	public class StandardToolsConfigComponent : ConfigurationApplicationComponent
	{
		private ToolSettings _settings;
		private bool _showNonCTModPixelValue;
		private bool _showCTRawPixelValue;
		private bool _showVoiPixelValue;

		public bool ShowNonCTModPixelValue
		{
			get { return _showNonCTModPixelValue; }
			set
			{
				if (_showNonCTModPixelValue != value)
				{
					_showNonCTModPixelValue = value;
					base.Modified = true;
					base.NotifyPropertyChanged("ShowNonCTModPixelValue");
				}
			}
		}

		public bool ShowCTRawPixelValue {
			get { return _showCTRawPixelValue; }
			set {
				if (_showCTRawPixelValue != value) {
					_showCTRawPixelValue = value;
					base.Modified = true;
					base.NotifyPropertyChanged("ShowCTRawPixelValue");
				}
			}
		}

		public bool ShowVOIPixelValue {
			get { return _showVoiPixelValue; }
			set {
				if (_showVoiPixelValue != value) {
					_showVoiPixelValue = value;
					base.Modified = true;
					base.NotifyPropertyChanged("ShowVOIPixelValue");
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_settings = ToolSettings.Default;

			try
			{
				_showNonCTModPixelValue = _settings.ShowNonCTModPixelValue;
			}
			catch
			{
				_showNonCTModPixelValue = false;
			}

			try
			{
				_showCTRawPixelValue = _settings.ShowCTRawPixelValue;
			}
			catch
			{
				_showCTRawPixelValue = false;
			}

			try
			{
				_showVoiPixelValue = _settings.ShowVOIPixelValue;
			}
			catch
			{
				_showVoiPixelValue = false;
			}
		}

		public override void Save()
		{
			_settings.ShowNonCTModPixelValue = _showNonCTModPixelValue;
			_settings.ShowCTRawPixelValue = _showCTRawPixelValue;
			_settings.ShowVOIPixelValue = _showVoiPixelValue;
		}

		public override void Stop()
		{
			_settings.Save();
			_settings = null;

			base.Stop();
		}
	}
}
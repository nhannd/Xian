using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard.Config
{
	[MenuAction("showCTPix", "probetool-dropdown/ShowCTPix", "ToggleShowCTPix")]
	[CheckedStateObserver("showCTPix", "ShowCTPix", "ShowCTPixChanged")]
	[Tooltip("showCTPix", "TooltipShowCTPix")]
	[GroupHint("showCTPix", "Tools.Image.Interrogation.Probe.Modality.CT.ShowPixel")]

	[MenuAction("showNonCTMod", "probetool-dropdown/ShowNonCTMod", "ToggleShowNonCTMod")]
	[CheckedStateObserver("showNonCTMod", "ShowNonCTMod", "ShowNonCTModChanged")]
	[Tooltip("showNonCTMod", "TooltipShowNonCTMod")]
	[GroupHint("showNonCTMod", "Tools.Image.Interrogation.Probe.Modality.NonCT.ShowMod")]

	[MenuAction("showVoiLut", "probetool-dropdown/ShowVoiLut", "ToggleShowVoiLut")]
	[CheckedStateObserver("showVoiLut", "ShowVoiLut", "ShowVoiLutChanged")]
	[Tooltip("showVoiLut", "TooltipShowVoiLut")]
	[GroupHint("showVoiLut", "Tools.Image.Interrogation.Probe.General.ShowVoiLut")]

	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ProbeSettingsTool : ImageViewerTool
	{
		private static event EventHandler _showCTPixChanged;
		private static event EventHandler _showNonCTModChanged;
		private static event EventHandler _showVoiLutChanged;
		private ToolSettings _settings;

		public override void Initialize()
		{
			base.Initialize();

			_settings = ToolSettings.Default;
		}

		protected override void Dispose(bool disposing)
		{
			_settings.Save();
			_settings = null;

			base.Dispose(disposing);
		}

		public event EventHandler ShowCTPixChanged
		{
			add { _showCTPixChanged += value; }
			remove { _showCTPixChanged -= value; }
		}

		public event EventHandler ShowNonCTModChanged
		{
			add { _showNonCTModChanged += value; }
			remove { _showNonCTModChanged -= value; }
		}

		public event EventHandler ShowVoiLutChanged
		{
			add { _showVoiLutChanged += value; }
			remove { _showVoiLutChanged -= value; }
		}

		public bool ShowCTPix
		{
			get
			{
				try
				{
					return _settings.ShowCTRawPixelValue;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				if (this.ShowCTPix != value)
				{
					_settings.ShowCTRawPixelValue = value;
					EventsHelper.Fire(_showCTPixChanged, this, new EventArgs());
				}
			}
		}

		public bool ShowNonCTMod
		{
			get
			{
				try
				{
					return _settings.ShowNonCTModPixelValue;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				if (this.ShowNonCTMod != value)
				{
					_settings.ShowNonCTModPixelValue = value;
					EventsHelper.Fire(_showNonCTModChanged, this, new EventArgs());
				}
			}
		}

		public bool ShowVoiLut
		{
			get
			{
				try
				{
					return _settings.ShowVOIPixelValue;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				if (this.ShowVoiLut != value)
				{
					_settings.ShowVOIPixelValue = value;
					EventsHelper.Fire(_showVoiLutChanged, this, new EventArgs());
				}
			}
		}

		public void ToggleShowCTPix()
		{
			this.ShowCTPix = !this.ShowCTPix;
		}

		public void ToggleShowNonCTMod()
		{
			this.ShowNonCTMod = !this.ShowNonCTMod;
		}

		public void ToggleShowVoiLut()
		{
			this.ShowVoiLut = !this.ShowVoiLut;
		}
	}
}
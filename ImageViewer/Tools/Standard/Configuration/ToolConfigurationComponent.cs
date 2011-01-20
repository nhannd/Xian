#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.Configuration
{
	[ExtensionPoint]
	public sealed class ToolConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ToolConfigurationComponentViewExtensionPoint))]
	public class ToolConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string Path = "StandardTools";

		private readonly BindingList<ToolOption> _options = new BindingList<ToolOption>();
		private ToolSettings _settings;

		public ToolConfigurationComponent() {}

		public IBindingList Options
		{
			get { return _options; }
		}

		private void OnOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.Modified = true;
		}

		public override void Start()
		{
			base.Start();

			_settings = ToolSettings.Default;

			ToolSettingsProfile profile = _settings.ToolSettingsProfile;
			if (profile != null)
			{
				foreach (string modality in StandardModalities.Modalities)
				{
					ToolOption option;
					_options.Add(option = new ToolOption(modality));
					option.PropertyChanged += new PropertyChangedEventHandler(OnOptionPropertyChanged);
					if (profile.HasSetting(modality))
					{
						option.AutoCineMultiframes = profile[modality].AutoCineMultiframes.GetValueOrDefault(false);
					}
				}
			}
		}

		public override void Save()
		{
			ToolSettingsProfile profile = _settings.ToolSettingsProfile;
			if (profile == null)
				_settings.ToolSettingsProfile = profile = new ToolSettingsProfile();
			foreach (ToolOption option in _options)
			{
				profile[option.Modality].AutoCineMultiframes = option.AutoCineMultiframes;
			}
			_settings.Save();
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}

		public class ToolOption : INotifyPropertyChanged
		{
			private event PropertyChangedEventHandler _propertyChanged;
			private readonly string _modality;
			private bool _autoCineMultiframes;

			public ToolOption(string modality)
			{
				_modality = modality;
			}

			public event PropertyChangedEventHandler PropertyChanged
			{
				add { _propertyChanged += value; }
				remove { _propertyChanged -= value; }
			}

			public string Modality
			{
				get { return _modality; }
			}

			public bool AutoCineMultiframes
			{
				get { return _autoCineMultiframes; }
				set
				{
					if (_autoCineMultiframes != value)
					{
						_autoCineMultiframes = value;
						this.OnPropertyChanged("AutoCineMultiframes");
					}
				}
			}

			protected virtual void OnPropertyChanged(string propertyName)
			{
				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
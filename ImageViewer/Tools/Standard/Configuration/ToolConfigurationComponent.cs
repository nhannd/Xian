#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
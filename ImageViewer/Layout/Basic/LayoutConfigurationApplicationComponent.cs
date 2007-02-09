using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// Extension point for views onto <see cref="LayoutSettingsApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class LayoutConfigurationApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// LayoutSettingsApplicationComponent class
	/// </summary>
	[AssociateView(typeof(LayoutConfigurationApplicationComponentViewExtensionPoint))]
	public class LayoutConfigurationApplicationComponent : ConfigurationApplicationComponent
	{
		private List<StoredLayoutConfiguration> _layoutConfigurations;

		public LayoutConfigurationApplicationComponent()
		{
		}

		public IList<StoredLayoutConfiguration> LayoutConfigurations
		{
			get
			{
				if (_layoutConfigurations == null)
				{
					_layoutConfigurations = new List<StoredLayoutConfiguration>(LayoutConfigurationSettings.Default.LayoutConfigurations);

					StoredLayoutConfiguration defaultConfiguration = _layoutConfigurations.Find(delegate(StoredLayoutConfiguration configuration) { return configuration.IsDefault; });

					//make sure there is one for each available modality, don't worry about the default - there will always be one provided by the settings class.
					foreach (string modality in StandardModalities.Modalities)
					{
						if (!_layoutConfigurations.Exists(delegate(StoredLayoutConfiguration configuration) { return configuration.Modality == modality; }))
							_layoutConfigurations.Add(new StoredLayoutConfiguration(modality, defaultConfiguration.ImageBoxRows, defaultConfiguration.ImageBoxColumns, defaultConfiguration.TileRows, defaultConfiguration.TileColumns));
					}


					_layoutConfigurations.Sort(new StoredLayoutConfigurationSortByModality());
					_layoutConfigurations.ForEach
						(
							delegate(StoredLayoutConfiguration configuration)
							{
								configuration.PropertyChanged +=
									delegate(object sender, PropertyChangedEventArgs e)
									{
										this.Modified = true;
									};
							}
						);
				
				}

				return _layoutConfigurations;
			}
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void Save()
		{
			LayoutConfigurationSettings.Default.LayoutConfigurations = _layoutConfigurations;
		}
	}
}

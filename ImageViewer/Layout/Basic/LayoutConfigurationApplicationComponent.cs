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

		/// <summary>
		/// Gets the maximum allowable rows for image boxes.
		/// </summary>
		public int MaximumImageBoxRows
		{
			get { return LayoutConfigurationSettings.MaximumImageBoxRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for image boxes.
		/// </summary>
		public int MaximumImageBoxColumns
		{
			get { return LayoutConfigurationSettings.MaximumImageBoxColumns; }
		}

		/// <summary>
		/// Gets the maximum allowable rows for tiles.
		/// </summary>
		public int MaximumTileRows
		{
			get { return LayoutConfigurationSettings.MaximumTileRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for tiles.
		/// </summary>
		public int MaximumTileColumns
		{
			get { return LayoutConfigurationSettings.MaximumTileColumns; }
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

		public static void Configure(IDesktopWindow desktopWindow)
		{
			ConfigurationDialog.Show(desktopWindow, LayoutConfigurationPageProvider.BasicLayoutConfigurationPath);
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

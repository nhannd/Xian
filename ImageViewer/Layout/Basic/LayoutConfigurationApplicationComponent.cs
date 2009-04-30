#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
	public sealed class LayoutConfigurationApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
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

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

using System.Drawing;
using System.ComponentModel;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumeration for the differnt possible sizes of toolstrip button items.
	/// </summary>
	public enum ToolStripSizeType { Small = 0, Medium, Large };

	//TODO: remove this class now that we can make settings classes public?

	/// <summary>
	/// Helper class that provides access to settings controlling display of toolstrips.
	/// </summary>
	public sealed class ToolStripSettingsHelper : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the default instance of the settings helper.
		/// </summary>
		public static readonly ToolStripSettingsHelper Default = new ToolStripSettingsHelper(ToolStripSettings.Default);

		private event PropertyChangedEventHandler _propertyChanged;
		private readonly ToolStripSettings _settings;

		internal ToolStripSettingsHelper(ToolStripSettings settings)
		{
			_settings = settings;
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		/// <summary>
		/// Refreshes the application settings property values from persistent storage.
		/// </summary>
		public void Reload()
		{
			_settings.Reload();
		}

		/// <summary>
		/// Restores the persisted application settings to their corresponding default properties.
		/// </summary>
		public void Reset()
		{
			_settings.Reset();
		}

		/// <summary>
		/// Stores the current values of the application settings properties.
		/// </summary>
		public void Save()
		{
			_settings.Save();
		}

		/// <summary>
		/// Upgrades application settings to reflect a more recent installation of the application.
		/// </summary>
		public void Upgrade()
		{
			_settings.Upgrade();
		}

		/// <summary>
		/// Gets or sets a value indicating if toolstrips longer than the window size should be wrapped.
		/// </summary>
		public bool WrapLongToolstrips
		{
			get { return _settings.WrapLongToolstrips; }
			set
			{
				if (_settings.WrapLongToolstrips != value)
				{
					_settings.WrapLongToolstrips = value;
					this.NotifyPropertyChanged("WrapLongToolstrips");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating the size of toolstrip button items.
		/// </summary>
		public ToolStripSizeType ToolStripSize
		{
			get { return _settings.ToolStripSize; }
			set
			{
				if (_settings.ToolStripSize != value)
				{
					_settings.ToolStripSize = value;
					this.NotifyPropertyChanged("ToolStripSize");
				}
			}
		}
	}

	[SettingsGroupDescription("Stores general settings for toolbars and menus.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class ToolStripSettings {}
}
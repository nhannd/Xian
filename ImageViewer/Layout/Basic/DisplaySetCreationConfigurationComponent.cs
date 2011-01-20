#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExtensionPoint]
	public sealed class DisplaySetCreationConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(DisplaySetCreationConfigurationComponentViewExtensionPoint))]
	public class DisplaySetCreationConfigurationComponent : ConfigurationApplicationComponent
	{
		private BindingList<StoredDisplaySetCreationSetting> _settings;

		public DisplaySetCreationConfigurationComponent()
		{
		}

		public override void Start()
		{
			Initialize();
			base.Start();
		}

		public override void Save()
		{
			DisplaySetCreationSettings.Default.Save(_settings);
		}

		private void Initialize()
		{
			List<StoredDisplaySetCreationSetting> sortedSettings = DisplaySetCreationSettings.Default.GetStoredSettings();
			sortedSettings = CollectionUtils.Sort(sortedSettings,
			                                      (setting1, setting2) => setting1.Modality.CompareTo(setting2.Modality));

			_settings = new BindingList<StoredDisplaySetCreationSetting>(sortedSettings);

			foreach (StoredDisplaySetCreationSetting setting in _settings)
				setting.PropertyChanged += OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.Modified = true;
		}

		public BindingList<StoredDisplaySetCreationSetting> Options
		{
			get { return _settings; }
		}
	}
}

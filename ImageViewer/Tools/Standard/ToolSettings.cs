#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[SettingsGroupDescription("Stores settings for standard tools.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class ToolSettings
	{
		private ToolModalityBehaviorCollection _cachedToolModalityBehavior;

		public ToolSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		protected override void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e)
		{
			_cachedToolModalityBehavior = null;

			base.OnSettingsLoaded(sender, e);
		}

		protected override void OnSettingChanging(object sender, SettingChangingEventArgs e)
		{
			if (e.SettingName == "ToolModalityBehavior")
				_cachedToolModalityBehavior = null;

			base.OnSettingChanging(sender, e);
		}

		public ToolModalityBehaviorCollection CachedToolModalityBehavior
		{
			get
			{
				if (_cachedToolModalityBehavior == null)
				{
					ToolModalityBehaviorCollection result;
					try
					{
						result = ToolModalityBehavior;
					}
					catch (Exception)
					{
						result = null;
					}
					_cachedToolModalityBehavior = result ?? new ToolModalityBehaviorCollection();
				}
				return _cachedToolModalityBehavior;
			}
		}
	}
}
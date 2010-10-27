#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Thumbnails.Configuration
{
	[ExtensionPoint]
	public sealed class ThumbnailsConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ThumbnailsConfigurationComponentViewExtensionPoint))]
	public class ThumbnailsConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string Path = "LabelThumbnails";

		private ThumbnailsSettings _settings;
		private bool _autoOpenThumbnails;

		public ThumbnailsConfigurationComponent() {}

		public bool AutoOpenThumbnails
		{
			get { return _autoOpenThumbnails; }
			set
			{
				if (_autoOpenThumbnails != value)
				{
					_autoOpenThumbnails = value;
					this.Modified = true;
					this.NotifyPropertyChanged("AutoOpenThumbnails");
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_settings = ThumbnailsSettings.Default;
			_autoOpenThumbnails = _settings.AutoOpenThumbnails;
		}

		public override void Save()
		{
			_settings.AutoOpenThumbnails = _autoOpenThumbnails;
			_settings.Save();
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}
	}
}
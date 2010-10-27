#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Externals.Config
{
	[ExtensionPoint]
	public sealed class ExternalsConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ExternalsConfigurationComponentViewExtensionPoint))]
	public class ExternalsConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string PATH = "ExternalApplications";

		private ExternalsConfigurationSettings _settings;
		private ExternalCollection _externals;

		public IDesktopWindow DesktopWindow
		{
			get { return base.Host.DesktopWindow; }
		}

		public ICollection<IExternal> Externals
		{
			get { return _externals; }
		}

		public void FlagModified()
		{
			this.Modified = true;
			this.NotifyPropertyChanged("Externals");
		}

		public override void Start()
		{
			base.Start();

			_settings = ExternalsConfigurationSettings.Default;

			try
			{
				_externals = _settings.Externals;
			}
			catch(Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Failed to load external application settings. The XML may be corrupt.");
			}

			if (_externals == null)
				_externals = new ExternalCollection();
		}

		public override void Stop()
		{
			_externals = null;
			_settings = null;

			base.Stop();
		}

		public override void Save()
		{
			try
			{
				_externals.Sort();
				_settings.Externals = _externals;
			}
			catch(Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Failed to save external application settings.");
			}

			_settings.Save();

			ExternalCollection.ReloadSavedExternals();
		}
	}
}
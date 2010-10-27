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

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionPoint]
	public sealed class KeyImageConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof (KeyImageConfigurationComponentViewExtensionPoint))]
	public class KeyImageConfigurationComponent : ConfigurationApplicationComponent
	{
		internal KeyImageConfigurationComponent()
		{
		}

		private bool _publishToDefaultServers;
		private bool _publishLocalToSourceAE;

		public bool PublishToDefaultServers
		{
			get { return _publishToDefaultServers; }
			set
			{
				if (value == _publishToDefaultServers)
					return;

				_publishToDefaultServers = value;
				base.Modified = true;
				NotifyPropertyChanged("PublishToDefaultServers");
			}
		}

		public bool PublishLocalToSourceAE
		{
			get { return _publishLocalToSourceAE; }
			set
			{
				if (value == _publishLocalToSourceAE)
					return;

				_publishLocalToSourceAE = value;
				base.Modified = true;
				NotifyPropertyChanged("PublishLocalToSourceAE");
			}
		}

		public override void Start()
		{
			PublishToDefaultServers = KeyImageSettings.Default.PublishToDefaultServers;
			PublishLocalToSourceAE = KeyImageSettings.Default.PublishLocalToSourceAE;

			base.Start();
		}

		public override void Save()
		{
			KeyImageSettings.Default.PublishToDefaultServers = PublishToDefaultServers;
			KeyImageSettings.Default.PublishLocalToSourceAE = PublishLocalToSourceAE;
			KeyImageSettings.Default.Save();
		}
	}
}
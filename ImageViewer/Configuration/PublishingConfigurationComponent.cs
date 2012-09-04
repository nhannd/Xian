#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionPoint]
	public sealed class PublishingConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof (PublishingConfigurationComponentViewExtensionPoint))]
	public class PublishingConfigurationComponent : ConfigurationApplicationComponent
	{
		internal PublishingConfigurationComponent()
		{
		}

		private bool _publishLocalToSourceAE;

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
			_publishLocalToSourceAE = DicomPublishingSettings.Default.SendLocalToStudySourceAE;

			base.Start();
		}

		public override void Save()
		{
            ApplicationSettingsExtensions.SetSharedPropertyValue(DicomPublishingSettings.Default, "SendLocalToStudySourceAE", PublishLocalToSourceAE);
		}
	}
}
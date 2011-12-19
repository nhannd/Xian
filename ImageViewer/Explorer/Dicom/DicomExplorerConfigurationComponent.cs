#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class DicomExplorerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(DicomExplorerConfigurationComponentViewExtensionPoint))]
	public class DicomExplorerConfigurationComponent : ConfigurationApplicationComponent
	{
		private bool _showPhoneticIdeographicNames = false;
		private bool _showNumberOfImagesInStudy = false;
		private bool _selectDefaultServerOnStartup = false;

		public DicomExplorerConfigurationComponent()
		{
		}

		public bool SelectDefaultServerOnStartup
		{
			get
			{
				return _selectDefaultServerOnStartup;
			}
			set
			{
				_selectDefaultServerOnStartup = value;
				NotifyPropertyChanged("SelectDefaultServerOnStartup");
				this.Modified = true;
			}
		}

		public bool ShowPhoneticIdeographicNames
		{
			get 
			{
				return _showPhoneticIdeographicNames; 
			}
			set 
			{ 
				_showPhoneticIdeographicNames = value;
				NotifyPropertyChanged("ShowPhoneticIdeographicNames");
				this.Modified = true;
			}
		}

		public bool ShowNumberOfImagesInStudy
		{
			get
			{
				return _showNumberOfImagesInStudy;
			}
			set
			{
				_showNumberOfImagesInStudy = value;
				NotifyPropertyChanged("ShowNumberOfImagesInStudy");
				this.Modified = true;
			}
		}

		public override void Start()
		{
			_selectDefaultServerOnStartup = DicomExplorerConfigurationSettings.Default.SelectDefaultServerOnStartup;
			_showPhoneticIdeographicNames = DicomExplorerConfigurationSettings.Default.ShowIdeographicName;
			_showNumberOfImagesInStudy = DicomExplorerConfigurationSettings.Default.ShowNumberOfImagesInStudy;
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void Save()
		{
			DicomExplorerConfigurationSettings.Default.SelectDefaultServerOnStartup = SelectDefaultServerOnStartup;
			DicomExplorerConfigurationSettings.Default.ShowIdeographicName = this.ShowPhoneticIdeographicNames;
			DicomExplorerConfigurationSettings.Default.ShowPhoneticName = this.ShowPhoneticIdeographicNames;
			DicomExplorerConfigurationSettings.Default.ShowNumberOfImagesInStudy = this.ShowNumberOfImagesInStudy;
			DicomExplorerConfigurationSettings.Default.Save();
		}
	}
}

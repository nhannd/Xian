using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomExplorerConfigurationApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DicomExplorerConfigurationApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DicomExplorerConfigurationApplicationComponent class
	/// </summary>
	[AssociateView(typeof(DicomExplorerConfigurationApplicationComponentViewExtensionPoint))]
	public class DicomExplorerConfigurationApplicationComponent : ConfigurationApplicationComponent
	{
		private bool _showPhoneticIdeographicNames = false;

		/// <summary>
		/// Constructor
		/// </summary>
		public DicomExplorerConfigurationApplicationComponent()
		{
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
				this.Modified = true;
			}
		}

		public override void Start()
		{
			_showPhoneticIdeographicNames = DicomExplorerConfigurationSettings.Default.ShowIdeographicName;
			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public override void Save()
		{
			DicomExplorerConfigurationSettings.Default.ShowIdeographicName = this.ShowPhoneticIdeographicNames;
			DicomExplorerConfigurationSettings.Default.ShowPhoneticName = this.ShowPhoneticIdeographicNames;
			DicomExplorerConfigurationSettings.Default.Save();
		}
	}
}

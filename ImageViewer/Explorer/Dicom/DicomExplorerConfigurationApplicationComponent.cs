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
		/// <summary>
		/// Constructor
		/// </summary>
		public DicomExplorerConfigurationApplicationComponent()
		{
		}

		public override void Start()
		{
			// TODO prepare the component for its live phase
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
			//throw new Exception("The method or operation is not implemented.");
		}
	}
}

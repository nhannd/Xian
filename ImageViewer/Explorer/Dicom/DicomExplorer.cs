using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionOf(typeof(HealthcareArtifactExplorerExtensionPoint))]
	public class DicomExplorer : IHealthcareArtifactExplorer
	{
		DicomExplorerComponent _component;

		public DicomExplorer()
		{

		}

		#region IHealthcareArtifactExplorer Members

		public string Name
		{
			get { return "DICOM"; }
		}

		public IApplicationComponent Component
		{
			get
			{
				if (_component == null)
					_component = new DicomExplorerComponent();

				return _component as IApplicationComponent;
			}
		}

		#endregion
	}
}

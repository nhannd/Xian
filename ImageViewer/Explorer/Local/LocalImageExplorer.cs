using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	[ExtensionOf(typeof(HealthcareArtifactExplorerExtensionPoint))]
	public class LocalImageExplorer : IHealthcareArtifactExplorer
	{
		LocalImageExplorerComponent _component;

		public LocalImageExplorer()
		{

		}

		#region IHealthcareArtifactExplorer Members

		public string Name
		{
			get { return "My Computer"; }
		}

		public IApplicationComponent Component
		{
			get
			{
				if (_component == null)
					_component = new LocalImageExplorerComponent();

				return _component as IApplicationComponent;
			}
		}

		#endregion

	}
}

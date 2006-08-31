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
		private SplitComponentContainer _splitComponentContainer;
		private AENavigatorComponent _aeNavigator;
		private StudyBrowserComponent _studyBrowser;

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
				if (_splitComponentContainer == null)
				{
					if (_aeNavigator == null)
						_aeNavigator = new AENavigatorComponent();

					if (_studyBrowser == null)
						_studyBrowser = new StudyBrowserComponent();

					SplitPane leftPane = new SplitPane("AE Navigator", _aeNavigator);
					SplitPane rightPane = new SplitPane("Study Browser", _studyBrowser);
					_splitComponentContainer = new SplitComponentContainer(leftPane, rightPane);
				}

				return _splitComponentContainer as IApplicationComponent;
			}
		}

		#endregion
	}
}

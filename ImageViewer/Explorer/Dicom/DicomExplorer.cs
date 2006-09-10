using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;

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
					CreateComponentContainer();

				return _splitComponentContainer as IApplicationComponent;
			}
		}

		#endregion

		private void CreateComponentContainer()
		{
			if (_aeNavigator == null)
				_aeNavigator = new AENavigatorComponent();

			_aeNavigator.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);

			if (_studyBrowser == null)
				_studyBrowser = new StudyBrowserComponent();

			_studyBrowser.SelectServer(GetLocalAE());

			SplitPane leftPane = new SplitPane("AE Navigator", _aeNavigator);
			SplitPane rightPane = new SplitPane("Study Browser", _studyBrowser);
			_splitComponentContainer = new SplitComponentContainer(leftPane, rightPane);
		}

		void OnSelectedServerChanged(object sender, EventArgs e)
		{
			// TODO: Remove this hack once AENavigator.Selected returns non
			// null for local machine
			if (_aeNavigator.ServerSelected == null)
				_studyBrowser.SelectServer(GetLocalAE());
			else
				_studyBrowser.SelectServer(_aeNavigator.ServerSelected);
		}

		// TODO: Remove this hack once AENavigator.Selected returns non
		// null for local machine
		private AEServer GetLocalAE()
		{
			return new AEServer("My Datastore", "", "", new HostName("localhost"), new AETitle("CC"), new ListeningPort(4000));
		}
	}
}

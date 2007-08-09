using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.OffisNetwork;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionOf(typeof(HealthcareArtifactExplorerExtensionPoint))]
	public class DicomExplorer : IHealthcareArtifactExplorer
	{
		private SplitComponentContainer _splitComponentContainer;
		private AENavigatorComponent _aeNavigator;
		private StudyBrowserComponent _studyBrowser;
		private SearchPanelComponent _searchPanel;

		public DicomExplorer()
		{

		}

		#region IHealthcareArtifactExplorer Members

		public string Name
		{
			get { return SR.TitleDicomExplorer; }
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

			if (_searchPanel == null)
				_searchPanel = new SearchPanelComponent(_studyBrowser);

			_studyBrowser.SelectServerGroup(_aeNavigator.SelectedServers);

			SplitPane leftPane = new SplitPane(SR.TitleNavigatorPane, _aeNavigator, 0.25f);
			SplitPane rightPane = new SplitPane(SR.TitleStudyBrowserPane, _studyBrowser, 0.75f);

			SplitComponentContainer bottomContainer = 
				new SplitComponentContainer(
				leftPane,
				rightPane, 
				SplitOrientation.Vertical);

			SplitPane topPane = new SplitPane(SR.TitleSearchPanelPane, _searchPanel, true);
			SplitPane bottomPane = new SplitPane(SR.TitleStudyNavigatorPane, bottomContainer, false);

			_splitComponentContainer = 
				new SplitComponentContainer(
				topPane, 
				bottomPane, 
				SplitOrientation.Horizontal);
		}

		void OnSelectedServerChanged(object sender, EventArgs e)
		{
			_studyBrowser.SelectServerGroup(_aeNavigator.SelectedServers);
		}
	}
}

#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Security.Policy;
using System.ServiceModel.Security;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.Configuration.ServerTree;
using System.Threading;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionOf(typeof(HealthcareArtifactExplorerExtensionPoint))]
	public class DicomExplorer : IHealthcareArtifactExplorer
	{
		private SplitComponentContainer _splitComponentContainer;
		private ServerTreeComponent _serverTreeComponent;
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

				return _splitComponentContainer;
			}
		}

		#endregion

		private void CreateComponentContainer()
		{
			if (_serverTreeComponent == null)
				_serverTreeComponent = new ServerTreeComponent();

			_serverTreeComponent.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);

			if (_studyBrowser == null)
				_studyBrowser = new StudyBrowserComponent();

			if (_searchPanel == null)
				_searchPanel = new SearchPanelComponent(_studyBrowser);

			_studyBrowser.SelectServerGroup(_serverTreeComponent.SelectedServers);

			try
			{
				_studyBrowser.Search();
			}
			catch (PolicyException)
			{
				//TODO: ignore this on startup or show message?
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
			}

			SplitPane leftPane = new SplitPane(SR.TitleServerTreePane, _serverTreeComponent, 0.25f);
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
			_studyBrowser.SelectServerGroup(_serverTreeComponent.SelectedServers);
		}
	}
}

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
using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.Automation;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.DesktopServices;
using System.Threading;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	#region Hosting

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class DicomExplorerAutomationServiceHostTool : DesktopServiceHostTool
	{
		public DicomExplorerAutomationServiceHostTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			ServiceHost host = new ServiceHost(typeof(DicomExplorerAutomation));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = AutomationNamespace.Value;

			return host;
		}
	}

	#endregion

	//Note: should the need arise, we could later allow the different explorers to be enumerated, but right now it's not necessary.

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = true, ConfigurationName = "DicomExplorerAutomation", Namespace = AutomationNamespace.Value)]
	public class DicomExplorerAutomation : IDicomExplorerAutomation
	{
		public DicomExplorerAutomation()
		{
		}

		#region IDicomExplorerAutomation Members

		public SearchLocalStudiesResult SearchLocalStudies(SearchLocalStudiesRequest request)
		{
			if (request == null)
				throw new FaultException("The request cannot be null.");
			
			if (!StudyStore.IsSupported)
				throw new FaultException<NoLocalStoreFault>(new NoLocalStoreFault(), "No local store was found.");

			DicomExplorerComponent explorerComponent = GetDicomExplorer();

			if (request.SearchCriteria == null)
				request.SearchCriteria = new DicomExplorerSearchCriteria();

			//Select the local server node.
			explorerComponent.ServerTreeComponent.SetSelection(explorerComponent.ServerTreeComponent.ServerTree.LocalServer);

			SynchronizationContext.Current.Post(
				delegate
				{
					var queryParams = new QueryParameters();
					PrepareQueryParameters(request.SearchCriteria, queryParams);
					explorerComponent.StudyBrowserComponent.Search(new List<QueryParameters> { queryParams });
				}, null); 

			return new SearchLocalStudiesResult();
		}

		public SearchRemoteStudiesResult SearchRemoteStudies(SearchRemoteStudiesRequest request)
		{
			if (request == null)
				throw new FaultException("The request cannot be null.");

			DicomExplorerComponent explorerComponent = GetDicomExplorer();

			if (request.SearchCriteria == null)
				request.SearchCriteria = new DicomExplorerSearchCriteria();

			string aeTitle = (request.AETitle ?? "").Trim();
			if (String.IsNullOrEmpty(aeTitle))
			{
				explorerComponent.SelectDefaultServers();
			}
			else
			{
				var server = CollectionUtils.SelectFirst(explorerComponent.ServerTreeComponent.ServerTree.FindChildServers(),
													 delegate(IServerTreeNode node)
													 {
														 if (node.IsServer)
                                                             return ((IServerTreeDicomServer)node).AETitle == aeTitle;

														 return false;
                                                     });
				if (server == null)
					throw new FaultException<ServerNotFoundFault>(new ServerNotFoundFault(), String.Format("Server '{0}' not found.", aeTitle));

				explorerComponent.ServerTreeComponent.SetSelection(server);
			}

			SynchronizationContext.Current.Post(
				delegate
					{
                        var queryParams = new QueryParameters();
						PrepareQueryParameters(request.SearchCriteria, queryParams);
						explorerComponent.StudyBrowserComponent.Search(new List<QueryParameters> { queryParams });
					}, null); 
			
			return new SearchRemoteStudiesResult();
		}

		#endregion

		private static DicomExplorerComponent GetDicomExplorer()
		{
			List<DicomExplorerComponent> explorerComponents = DicomExplorerComponent.GetActiveComponents();
			if (explorerComponents.Count == 0)
				throw new FaultException<DicomExplorerNotFoundFault>(new DicomExplorerNotFoundFault(), "No dicom explorers were found.");

			IDesktopWindow parentDesktopWindow;
			IDesktopObject parentShelfOrWorkspace;
			GetOwnerWindows(explorerComponents[0], out parentDesktopWindow, out parentShelfOrWorkspace);
			if (parentDesktopWindow != null) //activate the owner, if it was found.
				parentDesktopWindow.Activate();
			if (parentShelfOrWorkspace != null)
				parentShelfOrWorkspace.Activate();

			//there's only ever one of these right now anyway.
			return explorerComponents[0];
		}

		private static void GetOwnerWindows(DicomExplorerComponent explorerComponent, 
			out IDesktopWindow parentDesktopWindow, out IDesktopObject parentShelfOrWorkspace)
		{
			parentDesktopWindow = null;
			parentShelfOrWorkspace = null;

			foreach (IDesktopWindow desktopWindow in Application.DesktopWindows)
			{
				foreach (IWorkspace workspace in desktopWindow.Workspaces)
				{
					if (workspace.Component == explorerComponent)
					{
						parentDesktopWindow = desktopWindow;
						parentShelfOrWorkspace = workspace;
						return;
					}
				}

				foreach (IShelf shelf in desktopWindow.Shelves)
				{
					if (shelf.Component == explorerComponent)
					{
						parentDesktopWindow = desktopWindow;
						parentShelfOrWorkspace = shelf;
						return;
					}
				}
			}
		}

		private static void PrepareQueryParameters(DicomExplorerSearchCriteria searchCriteria, QueryParameters queryParams)
		{
		    /// TODO (CR Nov 2011): This is wrong - it's going to put the wildcards into the search fields, which it should not do.
			queryParams["PatientsName"] = QueryStringHelper.ConvertNameToSearchCriteria(searchCriteria.PatientsName);
			queryParams["ReferringPhysiciansName"] = QueryStringHelper.ConvertNameToSearchCriteria(searchCriteria.ReferringPhysiciansName);
			queryParams["PatientId"] = QueryStringHelper.ConvertStringToWildcardSearchCriteria(searchCriteria.PatientId, false, true);
			queryParams["AccessionNumber"] = QueryStringHelper.ConvertStringToWildcardSearchCriteria(searchCriteria.AccessionNumber, false, true);
			queryParams["StudyDescription"] = QueryStringHelper.ConvertStringToWildcardSearchCriteria(searchCriteria.StudyDescription, false, true);
			queryParams["StudyDate"] = DateRangeHelper.GetDicomDateRangeQueryString(searchCriteria.StudyDateFrom, searchCriteria.StudyDateTo);

			//At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
			//Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
			//underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
			//or by running multiple queries, one per modality specified (for example).
			queryParams["ModalitiesInStudy"] = DicomStringHelper.GetDicomStringArray(searchCriteria.Modalities ?? new List<string>());
		}
	}
}

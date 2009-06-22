#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.Services.Automation;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = true, ConfigurationName = "ViewerAutomation", Namespace = AutomationNamespace.Value)]
	public class ViewerAutomation: IViewerAutomation
	{
		private static readonly string _viewerNotFoundReason = "The specified viewer was not found.";

		public ViewerAutomation()
		{
		}

		private static IStudyRootQuery GetStudyRootQuery()
		{
			return Platform.GetService<IStudyRootQuery>();
		}

		#region IViewerAutomation Members

		public GetActiveViewersResult GetActiveViewers()
		{
			List<Viewer> viewers = new List<Viewer>();

			//The tool stores the viewer ids in order of activation, most recent first
			foreach (Guid viewerId in ViewerAutomationTool.GetViewerIds())
			{
				IImageViewer viewer = ViewerAutomationTool.GetViewer(viewerId);
				if (viewer != null && GetViewerWorkspace(viewer) != null)
					viewers.Add(new Viewer(viewerId, GetPrimaryStudyInstanceUid(viewer)));
			}

			if (viewers.Count == 0)
				throw new FaultException<NoActiveViewersFault>(new NoActiveViewersFault(), "No active viewers were found.");

			GetActiveViewersResult result = new GetActiveViewersResult();
			result.ActiveViewers = viewers;
			return result;
		}

		public GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request)
		{
			if (request == null)
			{
				string message = "The get viewer info request cannot be null.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			if (request.Viewer == null || request.Viewer.Identifier.Equals(Guid.Empty))
			{
				string message = "A valid viewer id must be specified.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerAutomationTool.GetViewer(request.Viewer.Identifier);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer ({0}) was not found, " +
									"likely because it has already been closed by the user.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Debug, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			GetViewerInfoResult result = new GetViewerInfoResult();
			result.AdditionalStudyInstanceUids = GetAdditionalStudyInstanceUids(viewer);
			return result;
		}

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			if (request == null)
			{
				string message = "The open studies request cannot be null.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			if (request.StudiesToOpen == null || request.StudiesToOpen.Count == 0)
			{
				string message = "At least one study must be specified.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			OpenStudiesResult result = new OpenStudiesResult();
			bool activateIfOpen = request.ActivateIfAlreadyOpen ?? true;

			try
			{
				string primaryStudyInstanceUid = request.StudiesToOpen[0].StudyInstanceUid;
				Workspace workspace = GetViewerWorkspace(primaryStudyInstanceUid);

				IImageViewer viewer;
				if (activateIfOpen && workspace != null)
				{
					viewer = ImageViewerComponent.GetAsImageViewer(workspace);
					workspace.Activate();
				}
				else
				{
					viewer = OpenStudies(request, primaryStudyInstanceUid);
				}

				Guid? viewerId = ViewerAutomationTool.GetViewerId(viewer);
				if (viewerId == null)
					throw new FaultException("Failed to retrieve the id of the specified viewer.");

				result.Viewer = new Viewer(viewerId.Value, primaryStudyInstanceUid);
				return result;
			}
			catch(FaultException)
			{
				throw;
			}
			catch(Exception e)
			{
				string message = "An unexpected error has occurred while attempting to open the study(s).";
				Platform.Log(LogLevel.Error, e, message);
				throw new FaultException(message);
			}
		}

		public void ActivateViewer(ActivateViewerRequest request)
		{
			if (request == null)
			{
				string message = "The activate viewer request cannot be null.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			if (request.Viewer == null || request.Viewer.Identifier.Equals(Guid.Empty))
			{
				string message = "A valid viewer id must be specified.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerAutomationTool.GetViewer(request.Viewer.Identifier);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer ({0}) was not found, " +
					"likely because it has already been closed by the user.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Debug, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			IWorkspace workspace = GetViewerWorkspace(viewer);
			if (workspace == null)
			{
				string message = String.Format("The specified viewer ({0}) was found, " + 
					"but does not appear to be hosted in one of the active workspaces.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			try
			{
				workspace.Activate();
			}
			catch(Exception e)
			{
				string message = String.Format("An unexpected error has occurred while attempting " + 
					"to activate the specified viewer ({0}).", request.Viewer.Identifier);
				Platform.Log(LogLevel.Error, e, message);
				throw new FaultException(message);
			}
		}

		public void CloseViewer(CloseViewerRequest request)
		{
			if (request == null)
			{
				string message = "The close viewer request cannot be null.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			if (request.Viewer == null || request.Viewer.Identifier.Equals(Guid.Empty))
			{
				string message = "A valid viewer id must be specified.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerAutomationTool.GetViewer(request.Viewer.Identifier);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer ({0}) was not found, " +
					"likely because it has already been closed by the user.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Debug, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			IWorkspace workspace = GetViewerWorkspace(viewer);
			if (workspace == null)
			{
				string message = String.Format("The specified viewer ({0}) was found, " +
					"but it does not appear to be hosted in one of the active workspaces.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			try
			{
				workspace.Close(UserInteraction.NotAllowed);
			}
			catch (Exception e)
			{
				string message = String.Format("An unexpected error has occurred while attempting " +
					"to close the specified viewer ({0}).", request.Viewer.Identifier);
				Platform.Log(LogLevel.Error, e, message);
				throw new FaultException(message);
			}
		}

		#endregion

		private static void CompleteOpenStudyInfo(List<OpenStudyInfo> openStudyInfo)
		{
			List<OpenStudyInfo> incomplete = CollectionUtils.Select(openStudyInfo,
						delegate(OpenStudyInfo info) { return String.IsNullOrEmpty(info.SourceAETitle); });
			
			//only go looking for studies if the source ae title is unspecified.
			if (incomplete.Count == 0)
				return;

			List<string> incompleteStudyUids = CollectionUtils.Map<OpenStudyInfo, string>(incomplete,
				delegate(OpenStudyInfo info) { return info.StudyInstanceUid; });

			using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(GetStudyRootQuery()))
			{
				IList<StudyRootStudyIdentifier> foundStudies = bridge.QueryByStudyInstanceUid(incompleteStudyUids);
				foreach (StudyRootStudyIdentifier study in foundStudies)
				{
					foreach (OpenStudyInfo info in openStudyInfo)
					{
						if (info.StudyInstanceUid == study.StudyInstanceUid)
						{
							info.SourceAETitle = study.RetrieveAeTitle;
							break;
						}
					}
				}
			}
		}

		private static IImageViewer OpenStudies(OpenStudiesRequest args, string primaryStudyInstanceUid)
		{
			CompleteOpenStudyInfo(args.StudiesToOpen);
			IDictionary<string, ApplicationEntity> serverMap = GetServerMap(args.StudiesToOpen);

			ImageViewerComponent viewer = new ImageViewerComponent(LayoutManagerCreationParameters.Extended);
			List<LoadStudyArgs> loadStudyArgs = new List<LoadStudyArgs>();

			foreach (OpenStudyInfo info in args.StudiesToOpen)
			{
				//None of the servers should be empty now, but if they are, assume local.
				//The worst that will happen is it will fail to load when it doesn't exist.
				ApplicationEntity server = null;
				string loader = "DICOM_LOCAL";

				if (!String.IsNullOrEmpty(info.SourceAETitle) && serverMap.ContainsKey(info.SourceAETitle))
				{
					server = serverMap[info.SourceAETitle];
					if (server != null)
						loader = "CC_STREAMING";
				}

				loadStudyArgs.Add(new LoadStudyArgs(info.StudyInstanceUid, server, loader));
			}

			Exception loadException = null;

			try
			{
				viewer.LoadStudies(loadStudyArgs);
			}
			catch (Exception e)
			{
				loadException = e;
			}

			if (primaryStudyInstanceUid != GetPrimaryStudyInstanceUid(viewer))
			{
				viewer.Dispose();
				string message = "Failed to open the primary study.";
				throw new FaultException<OpenStudiesFault>(new OpenStudiesFault(message), message);
			}
			else if (viewer.StudyTree.Patients.Count == 0)
			{
				viewer.Dispose();
				string message = "Failed to open any of the specified studies.";
				throw new FaultException<OpenStudiesFault>(new OpenStudiesFault(message), message);
			}
			else
			{
				ImageViewerComponent.Launch(viewer, new LaunchImageViewerArgs(ViewerLaunchSettings.WindowBehaviour));
			}

			//don't block waiting for the user to dismiss a dialog.
			if (loadException != null)
				SynchronizationContext.Current.Post(ReportLoadFailures, loadException);

			return viewer;
		}

		private static IDictionary<string, ApplicationEntity> GetServerMap(IEnumerable<OpenStudyInfo> openStudies)
		{
			Dictionary<string, ApplicationEntity> serverMap = new Dictionary<string, ApplicationEntity>();

			string localAE = ServerTree.GetClientAETitle();
			serverMap[localAE] = null;

			ServerTree serverTree = new ServerTree();
			List<IServerTreeNode> servers = serverTree.FindChildServers(serverTree.RootNode.ServerGroupNode);

			foreach (OpenStudyInfo info in openStudies)
			{
				if (!String.IsNullOrEmpty(info.SourceAETitle) && !serverMap.ContainsKey(info.SourceAETitle))
				{
					Server server = servers.Find(delegate(IServerTreeNode node)
								{
									return ((Server)node).AETitle == info.SourceAETitle;
								}) as Server;

					//only add streaming servers.
					if (server != null && server.IsStreaming)
					{
						serverMap[info.SourceAETitle] =
							new ApplicationEntity(server.Host, server.AETitle, server.Name, server.Port, 
							server.IsStreaming, server.HeaderServicePort, server.WadoServicePort);
					}
				}
			}

			return serverMap;
		}

		private static void ReportLoadFailures(object loadFailures)
		{
			ExceptionHandler.Report((Exception)loadFailures, Application.ActiveDesktopWindow);
		}

		private static string GetPrimaryStudyInstanceUid(IImageViewer viewer)
		{
			foreach (Patient patient in viewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					return study.StudyInstanceUID;
				}
			}

			return null;
		}

		private static List<string> GetAdditionalStudyInstanceUids(IImageViewer viewer)
		{
			List<string> studyInstanceUids = new List<string>();

			foreach (Patient patient in viewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					studyInstanceUids.Add(study.StudyInstanceUID);
				}
			}

			if (studyInstanceUids.Count > 0)
				studyInstanceUids.RemoveAt(0);

			return studyInstanceUids;
		}

		private static Workspace GetViewerWorkspace(IImageViewer viewer)
		{
			foreach (Workspace workspace in GetViewerWorkspaces())
			{
				IImageViewer workspaceViewer = ImageViewerComponent.GetAsImageViewer(workspace);
				if (viewer == workspaceViewer)
					return workspace;
			}

			return null;
		}

		private static Workspace GetViewerWorkspace(string primaryStudyUid)
		{
			foreach (Workspace workspace in GetViewerWorkspaces())
			{
				IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
				if (primaryStudyUid == GetPrimaryStudyInstanceUid(viewer))
					return workspace;
			}

			return null;
		}

		private static IEnumerable<Workspace> GetViewerWorkspaces()
		{
			foreach (DesktopWindow desktopWindow in Application.DesktopWindows)
			{
				foreach (Workspace workspace in desktopWindow.Workspaces)
				{
					IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
					if (viewer != null)
						yield return workspace;
				}
			}
		}
	}
}

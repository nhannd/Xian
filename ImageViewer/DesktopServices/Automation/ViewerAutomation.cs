using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.Services.Automation;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.StudyLocator;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
{
	#region OpenStudyResult

	internal class OpenStudyResult
	{
		private string _studyInstanceUid;
		private int _numberOfImagesLoaded;
		private int _numberOfImagesFailed;

		public OpenStudyResult()
		{
		}

		public OpenStudyResult(string studyInstanceUid)
			: this(studyInstanceUid, 0, 0)
		{
		}

		public OpenStudyResult(string studyInstanceUid, int loaded, int failed)
		{
			_studyInstanceUid = studyInstanceUid;
			_numberOfImagesLoaded = loaded;
			_numberOfImagesFailed = failed;
		}

		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		public int NumberOfImagesFailed
		{
			get { return _numberOfImagesFailed; }
			set { _numberOfImagesFailed = value; }
		}

		public int NumberOfImagesLoaded
		{
			get { return _numberOfImagesLoaded; }
			set { _numberOfImagesLoaded = value; }
		}
	}

	#endregion

	/// <summary>
	/// For internal use only.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = true, ConfigurationName = "ViewerAutomation", Namespace = AutomationNamespace.Value)]
	public class ViewerAutomation: IViewerAutomation
	{
		private static readonly string _sessionNotFoundReason = "The specified session was not found.";

		public ViewerAutomation()
		{
		}

		#region IViewerAutomation Members

		public GetActiveViewerSessionsResult GetActiveViewerSessions()
		{
			List<ViewerSession> sessions = new List<ViewerSession>();

			foreach (Workspace workspace in GetViewerWorkspaces())
			{
				IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
				if (viewer != null)
				{
					Guid? sessionGuid = ViewerSessionTool.GetSessionId(viewer);
					if (sessionGuid != null)
						sessions.Add(new ViewerSession((Guid) sessionGuid, GetPrimaryStudyInstanceUid(viewer)));
				}
			}

			if (sessions.Count == 0)
				throw new FaultException<NoActiveViewerSessionsFault>(new NoActiveViewerSessionsFault(), "No active viewer sessions were found.");

			GetActiveViewerSessionsResult result = new GetActiveViewerSessionsResult();
			result.ActiveViewerSessions = sessions;
			return result;
		}

		public GetViewerSessionInfoResult GetViewerSessionInfo(GetViewerSessionInfoRequest request)
		{
			if (request == null)
			{
				string message = "The get viewer session info request cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			if (request.ViewerSession == null || request.ViewerSession.Equals(Guid.Empty))
			{
				string message = "A valid viewer session id must be specified.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerSessionTool.GetViewer(request.ViewerSession.SessionId);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer session ({0}) was not found, " +
									"likely because it has already been closed by the user.", request.ViewerSession.SessionId);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerSessionNotFoundFault>(new ViewerSessionNotFoundFault(message), _sessionNotFoundReason);
			}

			GetViewerSessionInfoResult result = new GetViewerSessionInfoResult();
			result.AdditionalStudyInstanceUids = GetAdditionalStudyInstanceUids(viewer);
			return result;
		}

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			if (request == null)
			{
				string message = "The open studies request cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			if (request.StudiesToOpen == null || request.StudiesToOpen.Count == 0)
			{
				string message = "At least one study must be specified.";
				Platform.Log(LogLevel.Error, message);
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
					List<OpenStudyResult> loadFailures;
					viewer = CreateViewer(request, out loadFailures);

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
						OpenStudyHelper.Launch((ImageViewerComponent)viewer, ViewerLaunchSettings.WindowBehaviour);
					}

					//don't block waiting for the user to dismiss a dialog.
					if (loadFailures.Count > 0)
						SynchronizationContext.Current.Post(ReportLoadFailures, loadFailures);
				}

				Guid? viewerSession = ViewerSessionTool.GetSessionId(viewer);
				if (viewerSession == null)
					throw new FaultException("Failed to retrieve the session id for the specified viewer.");

				result.ViewerSession = new ViewerSession(viewerSession.Value, primaryStudyInstanceUid);
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

		public void ActivateViewerSession(ActivateViewerSessionRequest request)
		{
			if (request == null)
			{
				string message = "The activate viewer session request cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			if (request.ViewerSession == null || request.ViewerSession.Equals(Guid.Empty))
			{
				string message = "A valid viewer session id must be specified.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerSessionTool.GetViewer(request.ViewerSession.SessionId);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer session ({0}) was not found, " +
					"likely because it has already been closed by the user.", request.ViewerSession.SessionId);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerSessionNotFoundFault>(new ViewerSessionNotFoundFault(message), _sessionNotFoundReason);
			}

			IWorkspace workspace = GetViewerWorkspace(viewer);
			if (workspace == null)
			{
				string message = String.Format("The specified viewer session ({0}) was found, " + 
					"but does not appear to be hosted in one of the active workspaces.", request.ViewerSession.SessionId);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerSessionNotFoundFault>(new ViewerSessionNotFoundFault(message), _sessionNotFoundReason);
			}

			try
			{
				workspace.Activate();
			}
			catch(Exception e)
			{
				string message = String.Format("An unexpected error has occurred while attempting " + 
					"to activate the specified viewer session ({0}).", request.ViewerSession.SessionId);
				Platform.Log(LogLevel.Error, e, message);
				throw new FaultException(message);
			}
		}

		public void CloseViewerSession(CloseViewerSessionRequest request)
		{
			if (request == null)
			{
				string message = "The close viewer session request cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			if (request.ViewerSession == null || request.ViewerSession.Equals(Guid.Empty))
			{
				string message = "A valid viewer session id must be specified.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerSessionTool.GetViewer(request.ViewerSession.SessionId);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer session ({0}) was not found, " +
					"likely because it has already been closed by the user.", request.ViewerSession.SessionId);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerSessionNotFoundFault>(new ViewerSessionNotFoundFault(message), _sessionNotFoundReason);
			}

			IWorkspace workspace = GetViewerWorkspace(viewer);
			if (workspace == null)
			{
				string message = String.Format("The specified viewer session ({0}) was found, " +
					"but it does not appear to be hosted in one of the active workspaces.", request.ViewerSession.SessionId);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerSessionNotFoundFault>(new ViewerSessionNotFoundFault(message), _sessionNotFoundReason);
			}

			try
			{
				workspace.Close(UserInteraction.NotAllowed);
			}
			catch (Exception e)
			{
				string message = String.Format("An unexpected error has occurred while attempting " +
					"to close the specified viewer session ({0}).", request.ViewerSession.SessionId);
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

			string[] incompleteStudyUids = new string[incomplete.Count];
			int i = 0;
			foreach (OpenStudyInfo info in incomplete)
				incompleteStudyUids[i++] = info.StudyInstanceUid;

			IStudyLocator studyLocator = Platform.GetService<IStudyLocator>();

			try
			{
				IList<StudyRootStudyIdentifier> foundStudies = studyLocator.FindByStudyInstanceUid(incompleteStudyUids);
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
			finally
			{
				if (studyLocator is IDisposable)
					(studyLocator as IDisposable).Dispose();
			}
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
							new ApplicationEntity(server.Host, server.AETitle, server.Port, server.HeaderServicePort, server.WadoServicePort);
					}
				} 
			}

			return serverMap;
		}

		private static ImageViewerComponent CreateViewer(OpenStudiesRequest args, out List<OpenStudyResult> loadFailures)
		{
			//TODO: could change openstudyhelper to work this way and throw an exception rather than showing a dialog.
			loadFailures = new List<OpenStudyResult>();

			CompleteOpenStudyInfo(args.StudiesToOpen);
			IDictionary<string, ApplicationEntity> serverMap = GetServerMap(args.StudiesToOpen);

			ImageViewerComponent component = new ImageViewerComponent(LayoutManagerCreationParameters.Extended);
			foreach (OpenStudyInfo info in args.StudiesToOpen)
			{
				try
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

					component.LoadStudy(new LoadStudyArgs(info.StudyInstanceUid, server , loader));
				}
				catch(OpenStudyException openStudyException)
				{
					loadFailures.Add(new OpenStudyResult(info.StudyInstanceUid, 
						openStudyException.SuccessfulImages, 
						openStudyException.FailedImages));

					Platform.Log(LogLevel.Error, openStudyException, 
							"Failed to load {0} of {1} images for a study specified in the request ({2}).",
							openStudyException.FailedImages, 
							openStudyException.FailedImages + openStudyException.SuccessfulImages, info.StudyInstanceUid);
				}
				catch (Exception e)
				{
					//total failure.
					loadFailures.Add(new OpenStudyResult(info.StudyInstanceUid));
					Platform.Log(LogLevel.Error, e, "Failed to load a study specified in the request ({0}).", info.StudyInstanceUid);
				}
			}

			return component;
		}

		private static void ReportLoadFailures(object loadFailures)
		{
			List<OpenStudyResult> failures = (List<OpenStudyResult>)loadFailures;
			if (failures.Count == 0)
				return;

			List<OpenStudyResult> totalFailures = failures.FindAll(
				delegate(OpenStudyResult result) { return result.NumberOfImagesLoaded == 0; });

			StringBuilder messageBuilder = new StringBuilder();
			if (totalFailures.Count > 0)
			{
				if (totalFailures.Count == 1)
					messageBuilder.Append(SR.MessageFormatStudyLoadFailure);
				else
					messageBuilder.AppendFormat(SR.MessageFormatStudyLoadFailures, totalFailures.Count);
			}

			int partialFailures = failures.Count - totalFailures.Count;
			if (partialFailures != 0)
			{
				if (totalFailures.Count != 0)
					messageBuilder.AppendLine();

				if (partialFailures == 1)
					messageBuilder.AppendFormat(SR.MessagePartialStudyLoadFailure);
				else 
					messageBuilder.AppendFormat(SR.MessagePartialStudyLoadFailures, partialFailures);
			}

			messageBuilder.AppendLine();
			messageBuilder.Append(SR.MessagePleaseSeeLogs);

			Application.ActiveDesktopWindow.ShowMessageBox(messageBuilder.ToString(), MessageBoxActions.Ok);
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

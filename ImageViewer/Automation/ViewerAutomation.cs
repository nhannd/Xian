using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;
using System.Threading;
using System.Text;

namespace ClearCanvas.ImageViewer.Automation
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = true, IncludeExceptionDetailInFaults = true, Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
	public class ViewerAutomation: IViewerAutomation
	{
		private static readonly string _sessionNotFoundReason = "The specified session was not found.";

		public ViewerAutomation()
		{
		}

		#region IViewerAutomation Members

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			if (request == null)
			{
				string message = "The open studies request cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			if (request.StudyInstanceUids == null || request.StudyInstanceUids.Length == 0 || String.IsNullOrEmpty(request.StudyInstanceUids[0]))
			{
				string message = "At least one study instance uid must be specified.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			OpenStudiesResult result = new OpenStudiesResult();
			WindowBehaviour windowBehaviour = request.WindowBehaviour ?? ViewerLaunchSettings.WindowBehaviour;
			bool activateIfOpen = request.ActivateIfAlreadyOpen ?? true;

			try
			{
				Workspace workspace = GetViewerWorkspace(request.StudyInstanceUids[0]);
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

					if (viewer.StudyTree.Patients.Count == 0)
					{
						viewer.Dispose();
						string message = "Failed to open any of the specified studies.";
						throw new FaultException<OpenStudiesFault>(new OpenStudiesFault(message), message);
					}
					else
					{
						OpenStudyHelper.Launch((ImageViewerComponent)viewer, windowBehaviour);
					}

					//don't block waiting for the user to dismiss a dialog.
					if (loadFailures.Count > 0)
						SynchronizationContext.Current.Post(ReportLoadFailures, loadFailures);
				}

				Guid? viewerSession = ViewerSessionTool.GetSessionId(viewer);
				if (viewerSession == null)
					throw new ApplicationException("Failed to retrieve the session id for the specified viewer.");

				result.ViewerSession = new ViewerSession(viewerSession.Value);
				return result;
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

		private static ImageViewerComponent CreateViewer(OpenStudiesRequest args, out List<OpenStudyResult> loadFailures)
		{
			loadFailures = new List<OpenStudyResult>();

			ImageViewerComponent component = new ImageViewerComponent(LayoutManagerCreationParameters.Extended);
			foreach (string studyInstanceUid in args.StudyInstanceUids)
			{
				try
				{
					component.LoadStudy(new LoadStudyArgs(studyInstanceUid, null, "DICOM_LOCAL"));
				}
				catch(OpenStudyException openStudyException)
				{
					loadFailures.Add(new OpenStudyResult(studyInstanceUid, 
						openStudyException.SuccessfulImages, 
						openStudyException.FailedImages));

					Platform.Log(LogLevel.Error, openStudyException, 
							"Failed to load {0} of {1} images for a study specified in the request ({2}).",
							openStudyException.FailedImages, 
							openStudyException.FailedImages + openStudyException.SuccessfulImages, studyInstanceUid);
				}
				catch (Exception e)
				{
					//total failure.
					loadFailures.Add(new OpenStudyResult(studyInstanceUid));
					Platform.Log(LogLevel.Error, e, "Failed to load a study specified in the request ({0}).", studyInstanceUid);
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
				messageBuilder.AppendFormat(SR.MessageFormatStudyLoadFailures, totalFailures.Count);

			int partialFailures = failures.Count - totalFailures.Count;
			if (partialFailures != 0)
			{
				if (totalFailures.Count != 0)
					messageBuilder.AppendLine();

				messageBuilder.AppendFormat(SR.MessagePartialStudyLoadFailures, partialFailures);
			}

			messageBuilder.AppendLine();
			messageBuilder.Append(SR.MessagePleaseSeeLogs);

			Application.ActiveDesktopWindow.ShowMessageBox(messageBuilder.ToString(), MessageBoxActions.Ok);
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

				if (viewer.LogicalWorkspace.ImageSets.Count > 0 && viewer.LogicalWorkspace.ImageSets[0].DisplaySets.Count > 0)
				{
					IDisplaySet displaySet = viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0];
					if (displaySet.PresentationImages.Count > 0)
					{
						IPresentationImage image = displaySet.PresentationImages[0];
						if (image is IImageSopProvider)
						{
							if ((image as IImageSopProvider).ImageSop.StudyInstanceUID == primaryStudyUid)
								return workspace;
						}
					}
				}
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

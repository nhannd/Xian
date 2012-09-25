#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.Automation;
using ClearCanvas.Desktop;
using System;

namespace ClearCanvas.Ris.Client.ViewerIntegration
{
	[ExceptionPolicyFor(typeof(OpenStudyException))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	internal class FaultExceptionPolicy : IExceptionPolicy
	{
		#region IExceptionPolicy Members

		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			//We told the viewer automation service to handle showing contract fault
			//messages to the user, so we just log the exception.
			Platform.Log(LogLevel.Info, e);
		}

		#endregion
	}

	internal class QueryFailedException : Exception
	{
		public QueryFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	internal class OpenStudyException : Exception
	{
		public OpenStudyException(string accessionNumber, Exception innerException)
			: base(String.Format("Failed to open images with accession number {0}.", accessionNumber), innerException)
		{
		}
	}

	[ExtensionOf(typeof(ViewerIntegrationExtensionPoint))]
	public class ViewerAutomationIntegration : IViewerIntegration
	{
		public ViewerAutomationIntegration()
		{
		}

		private static IViewerAutomationBridge CreateBridge()
		{
			var bridge = new ViewerAutomationBridge(
				Platform.GetService<IViewerAutomation>(), 
				Platform.GetService<IStudyRootQuery>());

			bridge.OpenStudiesBehaviour.ActivateExistingViewer = true;
			//The viewer knows what the problem is better than us, so let it show the user an error.
			bridge.OpenStudiesBehaviour.ReportFaultToUser = true;
			return bridge;
		}

		#region IViewerIntegration Members

		public void Open(string accessionNumber)
		{
			try
			{
				using (IViewerAutomationBridge bridge = CreateBridge())
					bridge.OpenStudiesByAccessionNumber(accessionNumber);
			}
			catch (QueryNoMatchesException e)
			{
				throw new QueryFailedException(String.Format("No studies with accession number {0} could be found.", accessionNumber), e);
			}
			catch (FaultException<QueryFailedFault> e)
			{
				throw new QueryFailedException(String.Format("Query failed for studies matching accession number {0}.", accessionNumber), e);
			}
			catch (FaultException<StudyNotFoundFault> e) { throw new OpenStudyException(accessionNumber, e); }
			catch (FaultException<StudyOfflineFault> e) { throw new OpenStudyException(accessionNumber, e); }
			catch (FaultException<StudyNearlineFault> e) { throw new OpenStudyException(accessionNumber, e); }
			catch (FaultException<StudyInUseFault> e) { throw new OpenStudyException(accessionNumber, e); }
			catch (FaultException<OpenStudiesFault> e) { throw new OpenStudyException(accessionNumber, e); }
		}

		public void Close(string accessionNumber)
		{
			try
			{
				using (IViewerAutomationBridge bridge = CreateBridge())
				{
					foreach (Viewer viewer in bridge.GetViewersByAccessionNumber(accessionNumber))
						bridge.CloseViewer(viewer);
				}
			}
			catch (FaultException<NoViewersFault>)
			{
				// eat this exception, as it really just means that the user has closed all viewer workspaces
			}
		}

		public void Activate(string accessionNumber)
		{
			try
			{
				using (IViewerAutomationBridge bridge = CreateBridge())
				{
					foreach (Viewer viewer in bridge.GetViewersByAccessionNumber(accessionNumber))
					{
						bridge.ActivateViewer(viewer);
						return;
					}
				}
			}
			catch (FaultException<NoViewersFault>)
			{
				// eat this exception, as it really just means that the user has closed all viewer workspaces
			}
		}

		#endregion
	}
}

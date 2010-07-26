#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Services.Automation;
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
			using (IViewerAutomationBridge bridge = CreateBridge())
			{
				foreach (Viewer viewer in bridge.GetViewersByAccessionNumber(accessionNumber))
					bridge.CloseViewer(viewer);
			}
		}

		public void Activate(string accessionNumber)
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

		#endregion
	}
}

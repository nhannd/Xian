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

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	/// <summary>
	/// Service contract for automation of the viewer.
	/// </summary>
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IViewerAutomation", Namespace = AutomationNamespace.Value)]
	public interface IViewerAutomation
	{
		/// <summary>
		/// Gets all active <see cref="Viewer"/>s.
		/// </summary>
		/// <exception cref="FaultException{NoActiveViewersFault}">Thrown if there are no active <see cref="Viewer"/>s.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(NoActiveViewersFault))]
		GetActiveViewersResult GetActiveViewers();

		/// <summary>
		/// Gets information about the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request);

		/// <summary>
		/// Opens the requested studies in a <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{OpenStudiesFault}">Thrown if the primary study could not be opened.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(OpenStudiesFault))]
		OpenStudiesResult OpenStudies(OpenStudiesRequest request);

		/// <summary>
		/// Activates the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		void ActivateViewer(ActivateViewerRequest request);

		/// <summary>
		/// Closes the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		void CloseViewer(CloseViewerRequest request);
	}
}
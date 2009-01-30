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
using System.Collections.Generic;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.DicomServer
{
	/// <summary>
	/// WCF service interface to the Dicom Server.  The Dicom Server runs in a Shred, and the WCF interface allows
	/// external processes, such as the Viewer, to make requests via this interface.
	/// </summary>
	[ServiceContract(ConfigurationName = "IDicomServerService")]
	public interface IDicomServerService
	{
		/// <summary>
		/// Send studies to another Dicom Server.
		/// </summary>
		[Obsolete("Use IDicomSendService instead.")]
		[OperationContract(IsOneWay = true)]
		void Send(AEInformation destinationAEInformation, IEnumerable<string> studyInstanceUids);

		/// <summary>
		/// Performs a study level retrieve from another Dicom Server.  Series and Image level retrieves will not
		/// work using this method and are currently unsupported.
		/// </summary>
		/// <param name="sourceAEInformation">The Dicom server to retrieve from</param>
		/// <param name="studiesToRetrieve">The studies to retrieve.  At an absolute minimum, each <see cref="StudyInformation"/>
		/// object passed in must have the <see cref="StudyInformation.StudyInstanceUid"/> field populated.</param>
		[OperationContract(IsOneWay = true)]
		void RetrieveStudies(AEInformation sourceAEInformation, IEnumerable<StudyInformation> studiesToRetrieve);

		/// <summary>
		/// Retrieve the local Dicom Server configuration.
		/// </summary>
		/// <returns>The server's current configuration.</returns>
		[OperationContract]
		DicomServerConfiguration GetServerConfiguration();

		/// <summary>
		/// Set the Dicom Server configuration.  The Dicom server should automatically restart using the new settings,
		/// but it may not do so immediately.  It may wait until it is idle before making the changes.
		/// </summary>
		/// <param name="newConfiguration">The server's new configuration</param>
		[OperationContract]
		void UpdateServerConfiguration(DicomServerConfiguration newConfiguration);
	}
}

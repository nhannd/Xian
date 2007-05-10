using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

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
		/// Send an arbitrary set of Dicom Instances (by Uid) to another Dicom Server.  The Uids can be at the
		/// Study, Series or Image level.
		/// </summary>
		/// <param name="destinationAEInformation">The Dicom server to send to</param>
		/// <param name="uids">The Instances to send</param>
		[OperationContract]
		void Send(AEInformation destinationAEInformation, IEnumerable<string> uids);

		/// <summary>
		/// Performs a study level retrieve from another Dicom Server.  Series and Image level retrieves will not
		/// work using this method and are currently unsupported.
		/// </summary>
		/// <param name="sourceAEInformation">The Dicom server to retrieve from</param>
		/// <param name="studiesToRetrieve">The studies to retrieve.  At an absolute minimum, each <see cref="StudyInformation"/>
		/// object passed in must have the <see cref="StudyInformation.StudyInstanceUid"/> field populated.</param>
		[OperationContract]
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

#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Holds the parameters that specify the study to be loaded.
	/// </summary>
	public class LoadStudyArgs
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="studyInstanceUid"></param>
        /// <param name="server"></param>
		public LoadStudyArgs(string studyInstanceUid, IDicomServiceNode server)
		{
			Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUids");
            Platform.CheckForNullReference(server, "server");
            StudyInstanceUid = studyInstanceUid;
			Server = server;
		}

        /// <summary>
	    /// Gets the Study Instance UID of the study to be loaded.
	    /// </summary>
	    public string StudyInstanceUid { get; private set; }

	    //TODO (Marmot):IApplicationEntity?

        /// <summary>
        /// Gets the server from which the study can be loaded.
        /// </summary>
	    public IDicomServiceNode Server { get; private set; }
	}
}

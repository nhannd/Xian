#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Class used for returning result information when processing.  Used for importing and duplicate processing.
	/// </summary>
	public class DicomProcessingResult
	{
		public String AccessionNumber;
		public String StudyInstanceUid;
		public String SeriesInstanceUid;
		public String SopInstanceUid;
        public String SopClassUid;
        public bool Successful;
		public String ErrorMessage;
		public DicomStatus DicomStatus;
        public bool RestoreRequested;

		/// <summary>
		/// Indicates whether the sop being processed is a duplicate.
		/// </summary>
		/// <remarks>
		/// The result of the processing depends on the duplicate policy used.
		/// </remarks>
		public bool Duplicate;

		public void SetError(DicomStatus status, String message)
		{
			Successful = false;
			DicomStatus = status;
			ErrorMessage = message;
		}
	}
}

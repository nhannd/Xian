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

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Represents the context of image reconciliation scheduling when an image is processed by the 'StudyProcess' processor
	/// </summary>
	public class ReconcileImageContext
	{
		#region Private Members
		private DicomFile _file;
		private Study _existingStudy;
		private StudyStorageLocation _existStudyLocation;
		private ServerPartition _partition;
		private StudyIntegrityQueue _studyIntegrityQueue;
		private StudyHistory _history;
		private StudyStorageLocation _destinationStudyLocation;
		private bool _isDuplicate;
		private ImageSetDescriptor _imageSetDesc;
		private int _expirationTime;
		private string _storagePath;
		#endregion

		#region Public Properties

		public int ExpirationTime
		{
			get { return _expirationTime; }
			set { _expirationTime = value; }
		}

		public ImageSetDescriptor ImageSet
		{
			get
			{
				if (_imageSetDesc==null)
				{
					if (_file!=null)
					{
						_imageSetDesc = new ImageSetDescriptor(_file.DataSet);
					}
				}
				return _imageSetDesc;
			}
		}

		/// <summary>
		/// The dicom file to be reconciled
		/// </summary>
		public DicomFile File
		{
			get { return _file; }
			set { _file = value; }
		}

		/// <summary>
		/// The location of the existing study that has the same Study Instance Uid
		/// </summary>
		public StudyStorageLocation CurrentStudyLocation
		{
			get { return _existStudyLocation; }
			set { _existStudyLocation = value; }
		}

		/// <summary>
		/// The existing study that has the same Study Instance Uid
		/// </summary>
		public Study CurrentStudy
		{
			get { return _existingStudy; }
			set { _existingStudy = value; }
		}

		/// <summary>
		/// The Server Partition where the reconciliation takes place
		/// </summary>
		public ServerPartition Partition
		{
			get { return _partition; }
			set { _partition = value; }
		}

		/// <summary>
		/// The reconcile queue entry created as a result of the reconciliation
		/// </summary>
		public StudyIntegrityQueue ReconcileQueue
		{
			get { return _studyIntegrityQueue; }
			set { _studyIntegrityQueue = value; }
		}

		/// <summary>
		/// The <see cref="StudyHistory"/> record for the existing study.
		/// </summary>
		public StudyHistory History
		{
			get { return _history; }
			set { _history = value; }
		}

		public StudyStorageLocation DestinationStudyLocation
		{
			get { return _destinationStudyLocation; }
			set { _destinationStudyLocation = value; }
		}

		public string StoragePath
		{
			get { return _storagePath; }
			set { _storagePath = value; }
		}

		public bool IsDuplicate
		{
			get { return _isDuplicate; }
			set { _isDuplicate = value; }
		}

		#endregion
	}
}
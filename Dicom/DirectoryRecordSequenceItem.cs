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

using System;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// A class representing a DICOMDIR Directory Record
	/// </summary>
	public class DirectoryRecordSequenceItem : DicomSequenceItem
	{
		#region Private Members
		private DirectoryRecordSequenceItem _lowerLevelRecord;
		private DirectoryRecordSequenceItem _nextRecord;
		private uint _offset;
		#endregion

		/// <summary>
		/// The first directory record in the level below the current record.
		/// </summary>
		public DirectoryRecordSequenceItem LowerLevelRecord
		{
			get { return _lowerLevelRecord; }
			set { _lowerLevelRecord = value; }
		}

		/// <summary>
		/// The next directory record at the current level.
		/// </summary>
		public DirectoryRecordSequenceItem NextRecord
		{
			get { return _nextRecord; }
			set { _nextRecord = value; }
		}

		/// <summary>
		/// An offset to the directory record.  Used for reading and writing.
		/// </summary>
		internal uint Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}

		public override string ToString()
		{
			string toString = String.Empty;
			string recordType = base[DicomTags.DirectoryRecordType].GetString(0, "");
			if (recordType == DicomDirectoryWriter.DirectoryRecordTypeImage)
				toString = base[DicomTags.ReferencedSopInstanceUidInFile].GetString(0, "");
			else if (recordType == DicomDirectoryWriter.DirectoryRecordTypeSeries)
				toString = base[DicomTags.SeriesInstanceUid].GetString(0, "");
			else if (recordType == DicomDirectoryWriter.DirectoryRecordTypeStudy)
				toString = base[DicomTags.StudyInstanceUid].GetString(0, "");
			else if (recordType == DicomDirectoryWriter.DirectoryRecordTypePatient)
				toString = base[DicomTags.PatientId].GetString(0, "") + " " + base[DicomTags.PatientsName].GetString(0, "");

			return recordType + " " + toString;
		}
	}
}

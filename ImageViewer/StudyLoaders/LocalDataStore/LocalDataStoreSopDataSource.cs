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

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
	internal class LocalDataStoreSopDataSource : LocalSopDataSource
	{
		private readonly object _syncLock =  new object();
		private readonly ISopInstance _sop;

		public LocalDataStoreSopDataSource(ISopInstance sop)
			: base(sop.GetLocationUri().LocalDiskPath)
		{
			_sop = sop;
		}

		public override string TransferSyntaxUid
		{
			get { return _sop.TransferSyntaxUid; }
		}

		public override string StudyInstanceUid
		{
			get { return _sop.GetParentSeries().GetParentStudy().StudyInstanceUid; }
		}
		
		public override string SeriesInstanceUid
		{
			get { return _sop.GetParentSeries().SeriesInstanceUid; }
		}

		public override string SopInstanceUid
		{
			get { return _sop.SopInstanceUid; }
		}
		
		public override string SopClassUid
		{
			get { return _sop.SopClassUid; }
		}

		public override DicomAttribute GetDicomAttribute(uint tag)
		{
			//the _sop indexer is not thread-safe.
			lock (_syncLock)
			{
				if (_sop.IsStoredTag(tag))
					return _sop[tag];
			}

			return base[tag];
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			if (_sop.IsStoredTag(tag))
			{
				attribute = _sop[tag];
				if (!attribute.IsEmpty)
					return true;
			}

			return base.TryGetAttribute(tag, out attribute);
		}
	}
}

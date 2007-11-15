#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal sealed partial class DicomServerManager
	{
		internal class DicomQuerySession
		{
			private QueryKey _queryKey;
			private ReadOnlyQueryResultCollection _queryResults;
			private int _currentIndex;
			private ushort _dimseStatus;

			public DicomQuerySession(QueryKey queryKey, ReadOnlyQueryResultCollection queryResults)
			{
				_queryResults = queryResults;
				_queryKey = queryKey;
				_currentIndex = 0;
				_dimseStatus = (ushort)OffisDcm.STATUS_Pending;
			}

			#region Properties

			public QueryKey QueryKey
			{
				get { return _queryKey; }
			}

			public ReadOnlyQueryResultCollection QueryResults
			{
				get { return _queryResults; }
			}

			public int CurrentIndex
			{
				get { return _currentIndex; }
				set { _currentIndex = value; }
			}

			public ushort DimseStatus
			{
				get { return _dimseStatus; }
				set { _dimseStatus = value; }
			}

			#endregion
		}

		internal class DicomMoveSession
		{
			private SendParcel _parcel;
			private BackgroundTask _task;
			private int _progress;
			private ushort _status;

			public DicomMoveSession(SendParcel parcel, BackgroundTask task)
			{
				_parcel = parcel;
				_task = task;
				_progress = 0;
				_status = (ushort)OffisDcm.STATUS_Pending;
			}

			#region Properties

			public SendParcel Parcel
			{
				get { return _parcel; }
			}

			public BackgroundTask Task
			{
				get { return _task; }
			}

			public int Progress
			{
				get { return _progress; }
				set { _progress = value; }
			}

			public ushort Status
			{
				get { return _status; }
				set { _status = value; }
			}

			#endregion
		}

		internal class BackgroundTaskContainer
		{
			BackgroundTask _task;

			public BackgroundTaskContainer()
			{
			}

			public BackgroundTask Task
			{
				get { return _task; }
				set { _task = value; }
			}
		}
	}
}
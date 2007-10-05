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
			private int _progress = 0;

			public DicomMoveSession(SendParcel parcel, BackgroundTask task)
			{
				_parcel = parcel;
				_task = task;
				_progress = 0;
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
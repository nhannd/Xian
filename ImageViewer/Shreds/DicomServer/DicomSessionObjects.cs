using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class DicomQuerySession
    {
        ReadOnlyQueryResultCollection _queryResults;
        int _currentIndex;

        public DicomQuerySession(ReadOnlyQueryResultCollection queryResults)
        {
            _queryResults = queryResults;
            _currentIndex = 0;
        }

        #region Properties

        public ReadOnlyQueryResultCollection QueryResults
        {
            get { return _queryResults; }
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
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

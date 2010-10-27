#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    public class ActivityScheduling
    {
        private ActivityPerformer _performer;
        private DateTime? _startTime;
        private DateTime? _endTime;

        /// <summary>
        /// No-args constructor required by NHibernate.
        /// </summary>
        public ActivityScheduling()
        {

        }

        public ActivityPerformer Performer
        {
            get { return _performer; }
            set { _performer = value; }
        }

        public DateTime? StartTime
        {
            get { return _startTime; }
            internal set { _startTime = value; }
        }

        public DateTime? EndTime
        {
            get { return _endTime; }
            internal set { _endTime = value; }
        }

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		public void TimeShift(int minutes)
		{
			_startTime = _startTime.HasValue ? _startTime.Value.AddMinutes(minutes) : _startTime;
			_endTime = _endTime.HasValue ? _endTime.Value.AddMinutes(minutes) : _endTime;
		}
	}
}

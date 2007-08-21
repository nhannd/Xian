using System;
using System.Collections.Generic;
using System.Text;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;

namespace ClearCanvas.Samples.Google.Calendar
{
    public class CalendarEvent : IComparable<CalendarEvent> {

        private DateTime _startTime = DateTime.MaxValue;
        private DateTime _endTime = DateTime.MinValue;
        private string _title;
        private string _description;
        private string _location;

        internal CalendarEvent(EventEntry gcalEvent)
        {
            if (gcalEvent.Times.Count > 0)
            {
                _startTime = gcalEvent.Times[0].StartTime;
                _endTime = gcalEvent.Times[0].EndTime;
            }
            _title = gcalEvent.Title.Text;
            _description = gcalEvent.Content.Content;
            _location = gcalEvent.Locations.Count > 0 ? gcalEvent.Locations[0].ValueString : null;
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public string  Description
        {
            get { return _description; }
            set { _description = value; }
        }


        #region IComparable<CalendarEvent> Members

        public int CompareTo(CalendarEvent other) {
            return this.StartTime.CompareTo(other.StartTime);
        }

        #endregion
    }
}

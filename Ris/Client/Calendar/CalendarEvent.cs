using System;
using System.Collections.Generic;
using System.Text;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;

namespace ClearCanvas.Ris.Client.Calendar
{
    public class CalendarEvent : IComparable<CalendarEvent> {

        private EventEntry _gcalEvent;

        internal CalendarEvent(EventEntry gcalEvent) {
            _gcalEvent = gcalEvent;
        }

        public DateTime StartTime {
            get {
                return _gcalEvent.Times.Count > 0 ? _gcalEvent.Times[0].StartTime : DateTime.MaxValue;
            }
        }

        public DateTime EndTime {
            get {
                return _gcalEvent.Times.Count > 0 ? _gcalEvent.Times[0].EndTime : DateTime.MinValue;
            }
        }

        public string Title {
            get {
                return _gcalEvent.Title.Text;
            }
        }

        public string Location {
            get {
                return _gcalEvent.Locations.Count > 0 ? _gcalEvent.Locations[0].ValueString : null;
            }
        }

        #region IComparable<CalendarEvent> Members

        public int CompareTo(CalendarEvent other) {
            return this.StartTime.CompareTo(other.StartTime);
        }

        #endregion
    }
}

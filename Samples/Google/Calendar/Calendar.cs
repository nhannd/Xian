using System;
using System.Collections.Generic;
using System.Text;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Samples.Google.Calendar
{
    public class Calendar {

        const string CALENDAR_URI = "http://www.google.com/calendar/feeds/default/private/full";

        private CalendarService _service;

        public Calendar()
        {
            _service = new CalendarService("ClearCanvas-Workstation-1.0");
            _service.setUserCredentials("clearcanvas.demo", "clearcanvas1");
        }

        public CalendarEvent[] GetEvents(string fullTextQuery, DateTime? from, DateTime? until)
        {
            EventQuery query = new EventQuery();

            query.Uri = new Uri(CALENDAR_URI);

            query.Query = fullTextQuery;

            if (from != null)
            {
                query.StartTime = from.Value;
            }

            if (until != null)
            {
                query.EndTime = until.Value;
            }

            EventFeed calFeed = _service.Query(query);

            List<CalendarEvent> events = CollectionUtils.Map<EventEntry, CalendarEvent, List<CalendarEvent>>(calFeed.Entries,
                delegate(EventEntry e) { return new CalendarEvent(e); });
            events.Sort();
            return events.ToArray();
        }

        public CalendarEvent AddEvent(string title, string description, DateTime? start, DateTime? end)
        {
            EventEntry entry = new EventEntry();

            // Set the title and content of the entry.
            entry.Title.Text = title;
            entry.Content.Content = description;

            if (start != null || end != null)
            {
                When eventTime = new When();
                if(start != null)
                    eventTime.StartTime = (DateTime)start;

                if(end != null)
                    eventTime.EndTime = (DateTime)end;

                entry.Times.Add(eventTime);
            }

            // Send the request and receive the response:
            entry = _service.Insert(new Uri(CALENDAR_URI), entry) as EventEntry;

            return new CalendarEvent(entry);
        }
    }
}

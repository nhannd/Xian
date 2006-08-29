using System;
using System.Collections.Generic;
using System.Text;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;


namespace ClearCanvas.Ris.Client.Calendar
{
    public class Calendar {

        const string CALENDAR_URI = "http://www.google.com/calendar/feeds/jresnick@gmail.com/private-ca61e391382563ce142e9a009701f992/full";

        private CalendarService _service;

        public Calendar() {
            _service = new CalendarService("Mozilla");
        }

        public CalendarEvent[] GetEvents(DateTime? from, DateTime? until) {
            EventQuery query = new EventQuery();

            //           if (userName != null) {
            //               service.setUserCredentials("jresnick", "");
            //          }

            query.Uri = new Uri(CALENDAR_URI);

            if (from != null) {
                query.StartTime = from.Value;
            }

            if (until != null) {
                query.EndTime = until.Value;
            }

            EventFeed calFeed = _service.Query(query);

            Console.WriteLine("Query Feed Test " + query.Uri);
            Console.WriteLine("Post URI is:  " + calFeed.Post);

            List<CalendarEvent> events = new List<CalendarEvent>();
            foreach (EventEntry feedEntry in calFeed.Entries) {
                events.Add(new CalendarEvent(feedEntry));
            }

            events.Sort();

            return events.ToArray();
        }
    }
}

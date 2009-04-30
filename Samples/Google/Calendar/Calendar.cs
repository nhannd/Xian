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
using System.Collections.Generic;
using System.Text;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Samples.Google.Calendar
{
    /// <summary>
    /// Provides access to a Google calendar.
    /// </summary>
    public class Calendar
    {

        const string CALENDAR_URI = "http://www.google.com/calendar/feeds/default/private/full";

        private CalendarService _service;

        public Calendar()
        {
            _service = new CalendarService("ClearCanvas-Workstation-1.0");
            _service.setUserCredentials("clearcanvas.demo", "clearcanvas1");
        }

        /// <summary>
        /// Queries the calendar for events matching the specified criteria.
        /// </summary>
        /// <param name="fullTextQuery"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds a new event to the calendar using the specified information.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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

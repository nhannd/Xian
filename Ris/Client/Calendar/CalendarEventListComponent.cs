using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Calendar
{
    public class CalendarEventListComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(CalendarEventListComponentViewExtensionPoint))]
    public class CalendarEventListComponent : ApplicationComponent
    {
        private TableData<CalendarEvent> _eventData;

        public CalendarEventListComponent()
        {
            _eventData = new TableData<CalendarEvent>();
            _eventData.AddColumn<string>("Title", delegate(CalendarEvent e) { return e.Title; });
            _eventData.AddColumn<string>("Location", delegate(CalendarEvent e) { return e.Location; });
            _eventData.AddColumn<string>("Start", delegate(CalendarEvent e) { return e.StartTime; });
            _eventData.AddColumn<string>("End", delegate(CalendarEvent e) { return e.EndTime; });
        }

        public void SetEventData(CalendarEvent[] calEvents)
        {
            _eventData.Clear();
            foreach (CalendarEvent e in calEvents)
            {
                _eventData.Add(e);
            }
        }

        public TableData<CalendarEvent> EventData
        {
            get { return _eventData; }
        }

    }
}

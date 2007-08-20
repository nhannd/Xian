using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Calendar
{
    [ExtensionPoint]
    public class CalendarSearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(CalendarSearchComponentViewExtensionPoint))]
    public class CalendarSearchComponent : ApplicationComponent
    {
        private DateTime? _fromDate;
        private DateTime? _untilDate;

        public DateTime? UntilDate
        {
            get { return _untilDate; }
            set { _untilDate = value; }
        }
	
        public DateTime? FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; }
        }

        public void Search()
        {
            Calendar calendar = new Calendar();
            CalendarEvent[] events = calendar.GetEvents(_fromDate, _untilDate);

            CalendarEventListComponent resultComponent = new CalendarEventListComponent();
            resultComponent.SetEventData(events);

            ApplicationComponent.LaunchAsWorkspace(
                this.Host.DesktopWindow,
                resultComponent,
                "Calendar Events",
                null);
        }
	
    }
}

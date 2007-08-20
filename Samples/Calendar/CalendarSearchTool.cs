using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Calendar
{
    [MenuAction("show", "global-menus/Calendar/Search")]
    [ClickHandler("show", "Show")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class CalendarSearchTool : DesktopTool
    {
        private CalendarSearchComponent _component;

        public CalendarSearchTool()
        {
        }

        public void Show()
        {
            if (_component == null)
            {
                _component = new CalendarSearchComponent();

                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    _component,
                    "Search Calendar",
                    ShelfDisplayHint.DockLeft,
                    delegate(IApplicationComponent component) { _component = null; });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Calendar.View.WinForms
{
    [ExtensionOf(typeof(CalendarEventListComponentViewExtensionPoint))]
    public class CalendarEventListComponentView : WinFormsView, IApplicationComponentView
    {
        private CalendarEventListComponent _component;
        private CalendarEventListControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new CalendarEventListControl(_component);
                }
                return _control;
            }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (CalendarEventListComponent)component;
        }

        #endregion
    }
}

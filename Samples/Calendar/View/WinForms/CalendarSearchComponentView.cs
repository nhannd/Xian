using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Calendar.View.WinForms
{
    [ExtensionOf(typeof(CalendarSearchComponentViewExtensionPoint))]
    public class CalendarSearchComponentView : WinFormsView, IApplicationComponentView
    {
        private CalendarSearchComponent _component;
        private CalendarSearchControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new CalendarSearchControl(_component);
                }
                return _control;
            }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (CalendarSearchComponent)component;
        }

        #endregion
    }
}

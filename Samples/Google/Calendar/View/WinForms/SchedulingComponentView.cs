using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Samples.Google.Calendar.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="SchedulingComponent"/>
    /// </summary>
    [ExtensionOf(typeof(SchedulingComponentViewExtensionPoint))]
    public class SchedulingComponentView : WinFormsView, IApplicationComponentView
    {
        private SchedulingComponent _component;
        private SchedulingComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SchedulingComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SchedulingComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

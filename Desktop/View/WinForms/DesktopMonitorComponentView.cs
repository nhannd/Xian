using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DesktopMonitorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DesktopMonitorComponentViewExtensionPoint))]
    public class DesktopMonitorComponentView : WinFormsView, IApplicationComponentView
    {
        private DesktopMonitorComponent _component;
        private DesktopMonitorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DesktopMonitorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DesktopMonitorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ProtocolReasonComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ProtocolReasonComponentViewExtensionPoint))]
    public class ProtocolReasonComponentView : WinFormsView, IApplicationComponentView
    {
        private ProtocolReasonComponent _component;
        private ProtocolReasonComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ProtocolReasonComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ProtocolReasonComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ProtocollingComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ProtocollingComponentViewExtensionPoint))]
    public class ProtocollingComponentView : WinFormsView, IApplicationComponentView
    {
        private ProtocollingComponent _component;
        private ProtocollingComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ProtocollingComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ProtocollingComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

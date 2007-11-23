using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ProtocolGroupSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ProtocolGroupSummaryComponentViewExtensionPoint))]
    public class ProtocolGroupSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private ProtocolGroupSummaryComponent _component;
        private ProtocolGroupSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ProtocolGroupSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ProtocolGroupSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

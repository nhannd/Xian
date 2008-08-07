using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ReassignComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ReassignComponentViewExtensionPoint))]
    public class ReassignComponentView : WinFormsView, IApplicationComponentView
    {
        private ReassignComponent _component;
        private ReassignComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ReassignComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ReassignComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

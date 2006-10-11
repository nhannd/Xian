using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    [ExtensionOf(typeof(HL7QueuePreviewComponentViewExtensionPoint))]
    public class HL7QueuePreviewComponentView : WinFormsView, IApplicationComponentView
    {
        private HL7QueuePreviewComponent _component;
        private HL7QueuePreviewComponentControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (HL7QueuePreviewComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new HL7QueuePreviewComponentControl(_component);
                }
                return _control;
            }
        }

    }
}

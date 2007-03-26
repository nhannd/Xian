using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    [ExtensionOf(typeof(ReceiveQueueApplicationComponentViewExtensionPoint))]
    public class ReceiveQueueApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private ReceiveQueueApplicationComponent _component;
        private ReceiveQueueApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ReceiveQueueApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ReceiveQueueApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

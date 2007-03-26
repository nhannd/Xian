using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="SendQueueApplicationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(SendQueueApplicationComponentViewExtensionPoint))]
    public class SendQueueApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private SendQueueApplicationComponent _component;
        private SendQueueApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SendQueueApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SendQueueApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

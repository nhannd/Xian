using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    [ExtensionOf(typeof(DicomNetworkReceiveQueueApplicationComponentViewExtensionPoint))]
    public class DicomNetworkReceiveQueueApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomNetworkReceiveQueueApplicationComponent _component;
        private DicomNetworkReceiveQueueApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomNetworkReceiveQueueApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomNetworkReceiveQueueApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

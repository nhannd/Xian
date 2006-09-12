using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.Dicom.Services.View.WinForms
{
    [ExtensionOf(typeof(QueueDisplayComponentViewExtensionPoint))]
    class QueueDisplayComponentView : WinFormsView, IApplicationComponentView
    {
        #region Hancoded Members
        #region Private Members
        private QueueDisplayComponent _queueDisplay;
        private QueueDisplayControl _queueDisplayControl;
        #endregion
        #endregion


        public override object GuiElement
        {
            get 
            {
                if (null == _queueDisplayControl)
                    _queueDisplayControl = new QueueDisplayControl(_queueDisplay);

                return _queueDisplayControl;
            }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _queueDisplay = component as QueueDisplayComponent;
        }


        
        #endregion
    }
}

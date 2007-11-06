using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="MimeDocumentPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(MimeDocumentPreviewComponentViewExtensionPoint))]
    public class MimeDocumentPreviewComponentView : WinFormsView, IApplicationComponentView
    {
        private MimeDocumentPreviewComponent _component;
        private MimeDocumentPreviewComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (MimeDocumentPreviewComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new MimeDocumentPreviewComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

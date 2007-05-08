using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="BiographyDocumentComponent"/>
    /// </summary>
    [ExtensionOf(typeof(BiographyDocumentComponentViewExtensionPoint))]
    public class BiographyDocumentComponentView : WinFormsView, IApplicationComponentView
    {
        private BiographyDocumentComponent _component;
        private BiographyDocumentComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (BiographyDocumentComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new BiographyDocumentComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.Adt;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="XTechnologistDocumentationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(XTechnologistDocumentationComponentViewExtensionPoint))]
    public class XTechnologistDocumentationComponentView : WinFormsView, IApplicationComponentView
    {
        private XTechnologistDocumentationComponent _component;
        private XTechnologistDocumentationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (XTechnologistDocumentationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new XTechnologistDocumentationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

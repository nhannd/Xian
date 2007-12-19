using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TechnologistDocumentationOrderDetailsComponent"/>
    /// </summary>
    [ExtensionOf(typeof(TechnologistDocumentationOrderDetailsComponentViewExtensionPoint))]
    public class TechnologistDocumentationOrderDetailsComponentView : WinFormsView, IApplicationComponentView
    {
        private TechnologistDocumentationOrderDetailsComponent _component;
        private TechnologistDocumentationOrderDetailsComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (TechnologistDocumentationOrderDetailsComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new TechnologistDocumentationOrderDetailsComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

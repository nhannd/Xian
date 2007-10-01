using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TechnologistDocumentationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(TechnologistDocumentationComponentViewExtensionPoint))]
    public class TechnologistDocumentationComponentView : WinFormsView, IApplicationComponentView
    {
        //[ExtensionOf(typeof(TechnologistDocumentationComponent.OrderSummaryComponentViewExtensionPoint))]
        class OrderSummaryComponentView : DHtmlComponentView
        {
        }


        private TechnologistDocumentationComponent _component;
        private TechnologistDocumentationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (TechnologistDocumentationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new TechnologistDocumentationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

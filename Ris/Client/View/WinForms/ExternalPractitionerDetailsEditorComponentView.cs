using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ExternalPractitionerDetailsEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ExternalPractitionerDetailsEditorComponentViewExtensionPoint))]
    public class ExternalPractitionerDetailsEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ExternalPractitionerDetailsEditorComponent _component;
        private ExternalPractitionerDetailsEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ExternalPractitionerDetailsEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ExternalPractitionerDetailsEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

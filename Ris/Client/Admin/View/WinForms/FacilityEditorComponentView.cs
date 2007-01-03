using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="FacilityEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(FacilityEditorComponentViewExtensionPoint))]
    public class FacilityEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private FacilityEditorComponent _component;
        private FacilityEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (FacilityEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new FacilityEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

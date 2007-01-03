using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="LocationEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(LocationEditorComponentViewExtensionPoint))]
    public class LocationEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private LocationEditorComponent _component;
        private LocationEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LocationEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LocationEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

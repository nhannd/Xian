using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="RadiologistSelectionComponent"/>
    /// </summary>
    [ExtensionOf(typeof(RadiologistSelectionComponentViewExtensionPoint))]
    public class RadiologistSelectionComponentView : WinFormsView, IApplicationComponentView
    {
        private RadiologistSelectionComponent _component;
        private RadiologistSelectionComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RadiologistSelectionComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new RadiologistSelectionComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

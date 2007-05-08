using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientOrderHistoryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientOrderHistoryComponentViewExtensionPoint))]
    public class BiographyOrderHistoryComponentView : WinFormsView, IApplicationComponentView
    {
        private BiographyOrderHistoryComponent _component;
        private BiographyOrderHistoryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (BiographyOrderHistoryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new BiographyOrderHistoryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

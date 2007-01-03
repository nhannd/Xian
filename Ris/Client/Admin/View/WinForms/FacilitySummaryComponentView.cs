using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="FacilitySummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(FacilitySummaryComponentViewExtensionPoint))]
    public class FacilitySummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private FacilitySummaryComponent _component;
        private FacilitySummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (FacilitySummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new FacilitySummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

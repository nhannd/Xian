using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="VisitLocationsSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(VisitLocationsSummaryComponentViewExtensionPoint))]
    public class VisitLocationsSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private VisitLocationsSummaryComponent _component;
        private VisitLocationsSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (VisitLocationsSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new VisitLocationsSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="NoteSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(NoteSummaryComponentViewExtensionPoint))]
    public class NoteSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private NoteSummaryComponent _component;
        private NoteSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (NoteSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new NoteSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

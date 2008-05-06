using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms view onto <see cref="CannedTextSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(CannedTextSummaryComponentViewExtensionPoint))]
    public class CannedTextSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private CannedTextSummaryComponent _component;
        private CannedTextSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (CannedTextSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new CannedTextSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

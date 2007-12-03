using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="LinkedInterpretationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(LinkedInterpretationComponentViewExtensionPoint))]
    public class LinkedInterpretationComponentView : WinFormsView, IApplicationComponentView
    {
        private LinkedInterpretationComponent _component;
        private LinkedInterpretationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LinkedInterpretationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LinkedInterpretationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

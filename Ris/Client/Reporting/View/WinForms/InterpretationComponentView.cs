using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="InterpretationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(InterpretationComponentViewExtensionPoint))]
    public class InterpretationComponentView : WinFormsView, IApplicationComponentView
    {
        private InterpretationComponent _component;
        private InterpretationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (InterpretationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new InterpretationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

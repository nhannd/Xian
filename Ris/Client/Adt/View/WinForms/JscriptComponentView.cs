using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="JscriptComponent"/>
    /// </summary>
    [ExtensionOf(typeof(JscriptComponentViewExtensionPoint))]
    public class JscriptComponentView : WinFormsView, IApplicationComponentView
    {
        private JscriptComponent _component;
        private JscriptComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (JscriptComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new JscriptComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

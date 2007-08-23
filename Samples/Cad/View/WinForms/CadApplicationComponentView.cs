using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Samples.Cad;

namespace ClearCanvas.Samples.Cad.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="CadApplicationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(CadApplicationComponentViewExtensionPoint))]
    public class CadApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private CadApplicationComponent _component;
        private CadApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (CadApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new CadApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

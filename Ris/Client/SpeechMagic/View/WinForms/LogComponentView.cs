using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="LogComponent"/>
    /// </summary>
    [ExtensionOf(typeof(LogComponentViewExtensionPoint))]
    public class LogComponentView : WinFormsView, IApplicationComponentView
    {
        private LogComponent _component;
        private LogComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LogComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LogComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    [ExtensionOf(typeof(DHtmlComponentViewExtensionPoint))]
    public class DHtmlComponentView : WinFormsView, IApplicationComponentView
    {
        private DHtmlComponent _component;
        private DHtmlComponentControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DHtmlComponent) component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DHtmlComponentControl(_component);
                }
                return _control;
            }
        }

    }
}

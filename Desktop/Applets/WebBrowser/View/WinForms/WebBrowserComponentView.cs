using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Applets.WebBrowser;

namespace ClearCanvas.Desktop.Applets.WebBrowser.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="BrowserComponent"/>
    /// </summary>
    [ExtensionOf(typeof(BrowserComponentViewExtensionPoint))]
    public class WebBrowserComponentView : WinFormsView, IApplicationComponentView
    {
        private WebBrowserComponent _component;
        private WebBrowserComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (WebBrowserComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new WebBrowserComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.ExtensionBrowser.View.WinForms
{
    /// <summary>
    /// WinForms implemenation of a view onto <see cref="ExtensionBrowserComponent"/>.  This class
    /// delegates all of the real work to the <see cref="ExtensionBrowserControl"/> class.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ExtensionBrowserComponentViewExtensionPoint))]
    public class ExtensionBrowserView : WinFormsView, IApplicationComponentView
    {
        private ExtensionBrowserComponent _browser;
        private ExtensionBrowserControl _browserControl;
       
        public ExtensionBrowserView()
        {
        }

        /// <summary>
        /// Implementation of <see cref="IApplicationComponentView.SetComponent"/>
        /// </summary>
        /// <param name="component"></param>
        public void SetComponent(IApplicationComponent component)
        {
            _browser = (ExtensionBrowserComponent)component;
        }

        /// <summary>
        /// Implementation of <see cref="IView.GuiElement"/>.  Gets the WinForms
        /// element that provides the UI for this view.
        /// </summary>
        public override object GuiElement
        {
            get
            {
                if (_browserControl == null)
                {
                    _browserControl = new ExtensionBrowserControl(_browser);
                }
                return _browserControl;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// WinForms implementation of the <see cref="LayoutComponentViewExtensionPoint"/> extension point.
    /// The actual user-interface is implemented by <see cref="LayoutControl"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(LayoutComponentViewExtensionPoint))]
    public class LayoutComponentView : WinFormsView, IApplicationComponentView
    {
        private Control _control;
        private LayoutComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public LayoutComponentView()
        {
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LayoutComponent)component;
        }

        #endregion

        /// <summary>
        /// Overridden to return an instance of <see cref="LayoutControl"/>
        /// </summary>
        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LayoutControl(_component);
                }
                return _control;
            }
        }
    }
}

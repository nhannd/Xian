using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="FolderExplorerGroupComponent"/>
    /// </summary>
    [ExtensionOf(typeof(FolderExplorerGroupComponentViewExtensionPoint))]
    public class FolderExplorerGroupComponentView : WinFormsView, IApplicationComponentView
    {
        private FolderExplorerGroupComponent _component;
        private FolderExplorerGroupComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (FolderExplorerGroupComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new FolderExplorerGroupComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

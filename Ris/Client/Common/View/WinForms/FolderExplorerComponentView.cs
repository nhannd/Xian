using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Common.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="WorklistExplorerComponent"/>
    /// </summary>
    [ExtensionOf(typeof(FolderExplorerComponentViewExtensionPoint))]
    public class FolderExplorerComponentView : WinFormsView, IApplicationComponentView
    {
        private FolderExplorerComponent _component;
        private FolderExplorerComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (FolderExplorerComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new FolderExplorerComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

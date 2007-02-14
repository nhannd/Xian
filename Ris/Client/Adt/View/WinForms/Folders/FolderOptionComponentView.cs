using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.Folders.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="FolderOptionComponent"/>
    /// </summary>
    [ExtensionOf(typeof(FolderOptionComponentViewExtensionPoint))]
    public class FolderOptionComponentView : WinFormsView, IApplicationComponentView
    {
        private FolderOptionComponent _component;
        private FolderOptionComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (FolderOptionComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new FolderOptionComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

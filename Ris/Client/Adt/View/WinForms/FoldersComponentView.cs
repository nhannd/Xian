using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="FoldersComponent"/>
    /// </summary>
    [ExtensionOf(typeof(FoldersComponentViewExtensionPoint))]
    public class FoldersComponentView : WinFormsView, IApplicationComponentView
    {
        private FoldersComponent _component;
        private FoldersComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (FoldersComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new FoldersComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

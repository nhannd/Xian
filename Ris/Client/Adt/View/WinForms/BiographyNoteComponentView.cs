using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="BiographyNoteComponent"/>
    /// </summary>
    [ExtensionOf(typeof(BiographyNoteComponentViewExtensionPoint))]
    public class BiographyNoteComponentView : WinFormsView, IApplicationComponentView
    {
        private BiographyNoteComponent _component;
        private BiographyNoteComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (BiographyNoteComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new BiographyNoteComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

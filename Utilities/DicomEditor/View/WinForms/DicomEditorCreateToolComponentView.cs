using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Utilities.DicomEditor.Tools;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomEditorCreateToolComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DicomEditorCreateToolComponentViewExtensionPoint))]
    public class DicomEditorCreateToolComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomEditorCreateToolComponent _component;
        private DicomEditorCreateToolComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomEditorCreateToolComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomEditorCreateToolComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    [ExtensionOf(typeof(DicomEditorComponentViewExtensionPoint))]
    public class DicomEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomEditorComponent _component;
        private DicomEditorControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomEditorControl(_component);
                }
                return _control;
            }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomEditorComponent)component;
        }

        #endregion
    }
}

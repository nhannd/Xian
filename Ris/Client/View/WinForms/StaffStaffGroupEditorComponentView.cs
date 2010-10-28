#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="StaffStaffGroupEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(StaffStaffGroupEditorComponentViewExtensionPoint))]
    public class StaffStaffGroupEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private StaffStaffGroupEditorComponent _component;
        private StaffStaffGroupEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (StaffStaffGroupEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new StaffStaffGroupEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

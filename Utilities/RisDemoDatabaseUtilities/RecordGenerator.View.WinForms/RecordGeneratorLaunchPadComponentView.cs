#region License

// Copyright (c) 2011, ClearCanvas Inc.
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

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="RecordGeneratorLaunchPadComponent"/>
    /// </summary>
    [ExtensionOf(typeof(RecordGeneratorLaunchPadComponentViewExtensionPoint))]
    public class RecordGeneratorLaunchPadComponentView : WinFormsView, IApplicationComponentView
    {
        private RecordGeneratorLaunchPadComponent _component;
        private RecordGeneratorLaunchPadComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RecordGeneratorLaunchPadComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new RecordGeneratorLaunchPadComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

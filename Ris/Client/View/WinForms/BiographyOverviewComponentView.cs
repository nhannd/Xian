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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="BiographyOverviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(BiographyOverviewComponentViewExtensionPoint))]
    public class BiographyOverviewComponentView : WinFormsView, IApplicationComponentView
    {
        private BiographyOverviewComponent _component;
        private BiographyOverviewComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (BiographyOverviewComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new BiographyOverviewComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

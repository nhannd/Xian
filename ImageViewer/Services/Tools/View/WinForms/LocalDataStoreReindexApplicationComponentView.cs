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

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="LocalDataStoreReindexApplicationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(LocalDataStoreReindexApplicationComponentViewExtensionPoint))]
    public class LocalDataStoreReindexApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private LocalDataStoreReindexApplicationComponent _component;
        private LocalDataStoreReindexApplicationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LocalDataStoreReindexApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LocalDataStoreReindexApplicationComponentControl(_component.Reindexer){StartEnabled = true};
                }
                return _control;
            }
        }
    }
}

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

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="SearchPanelComponent"/>
    /// </summary>
    [ExtensionOf(typeof(SearchPanelComponentViewExtensionPoint))]
    public class SearchPanelComponentView : WinFormsView, IApplicationComponentView
    {
        private SearchPanelComponent _component;
        private SearchPanelComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SearchPanelComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SearchPanelComponentControl(_component);
                }
                return _control;
            }
        }
    }
}

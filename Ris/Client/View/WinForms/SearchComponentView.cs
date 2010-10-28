#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    [ExtensionOf(typeof(SearchComponentViewExtensionPoint))]
    public class SearchComponentView : WinFormsView, IApplicationComponentView
    {
        private SearchComponent _component;
        private SearchComponentControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SearchComponentControl(_component);
                }
                return _control;
            }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SearchComponent)component;
        }

        #endregion

    }
}

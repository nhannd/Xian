#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Utilities.WebViewerAnalysisTools.LossyCompression
{
    /// <summary>
    /// Extension point for views onto <see cref="CompressionAnalysisComponent"/>.
    /// </summary>
    [ExtensionPoint]
    public sealed class CompressionAnalysisComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionOf(typeof(CompressionAnalysisComponentViewExtensionPoint))]
    public class CompressionAnalysisComponentView : WinFormsView, IApplicationComponentView
    {
        private CompressionAnalysisComponent _component;
        private CompressionAnalysisComponentControl _control;

        #region IApplicationComponentView Members

        /// <summary>
        /// Called by the host to assign this view to a component.
        /// </summary>
        public void SetComponent(IApplicationComponent component)
        {
            _component = (CompressionAnalysisComponent)component;
        }

        #endregion

        /// <summary>
        /// Gets the underlying GUI component for this view.
        /// </summary>
        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new CompressionAnalysisComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
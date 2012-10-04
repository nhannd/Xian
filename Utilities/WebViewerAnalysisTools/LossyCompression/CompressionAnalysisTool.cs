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
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.Utilities.WebViewerAnalysisTools.LossyCompression
{
    [Desktop.Actions.MenuAction("analyze", "imageviewer-contextmenu/Compression Analysis", "Analyze")]
    [Desktop.Actions.ButtonAction("analyze", "global-toolbars/ToolbarStandard/ToolbarCompressionAnalysis", "Analyze", KeyStroke = XKeys.Space)]
    [Desktop.Actions.Tooltip("analyze", "TooltipCompressionAnalysis")]
    [Desktop.Actions.IconSet("analyze", IconScheme.Colour, "Icons.CreateKeyImageToolSmall.png", "Icons.CreateKeyImageToolMedium.png", "Icons.CreateKeyImageToolLarge.png")]
    [Desktop.Actions.EnabledStateObserver("analyze", "Enabled", "EnabledChanged")]
    
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    internal class LossyCompressionAnalysisTool : ImageViewerTool
    {
        #region Private Fields

        #endregion

        #region Overrides

        #endregion

        #region Methods

        public void Analyze()
        {
            CompressionAnalysisComponent.Launch(Context.DesktopWindow, Context.Viewer.SelectedPresentationImage);
        }


        #endregion
    }

}
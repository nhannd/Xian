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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuLocateOnDisk", "Activate")]
	[Tooltip("activate", "TooltipLocateOnDisk")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class LocateOnDiskTool : ImageViewerTool
    {
        public LocateOnDiskTool()
        {
        }

        public void Activate()
        {
            if (this.SelectedPresentationImage == null)
                return;

            IImageSopProvider image = this.SelectedPresentationImage as IImageSopProvider;
			if (image == null)
				return;

        	ILocalSopDataSource localSource = image.ImageSop.DataSource as ILocalSopDataSource;
			if (localSource == null)
			{
				base.Context.DesktopWindow.ShowMessageBox(SR.MessageUnableToLocateNonLocalImage, MessageBoxActions.Ok);
				return;
			}

			System.Diagnostics.Process.Start("explorer.exe", "/n,/select," + localSource.Filename);
        }
    }
}

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

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuLocateOnDisk")]
	[Tooltip("activate", "Locates the selected image(s) on disk")]
	[ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class LocateOnDiskTool : ImageViewerTool
    {
        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public LocateOnDiskTool()
        {
        }


        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// You may change the name of this method as desired, but be sure to change the
        /// ClickHandler attribute accordingly.
        /// </summary>
        public void Activate()
        {
            if (this.SelectedPresentationImage == null)
                return;

            StandardPresentationImage image = this.SelectedPresentationImage as StandardPresentationImage;
			DicomFile dicomFile = image.ImageSop.NativeDicomObject as DicomFile;
			if (dicomFile == null)
				return;

			Platform.OpenFileBrowser(dicomFile.Filename);
        }
    }
}

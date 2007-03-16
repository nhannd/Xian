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
    [MenuAction("apply", "global-menus/MenuTools/Standard/MenuStandardLocateOnDisk")]
    [Tooltip("apply", "Locates the selected image(s) on disk")]
    [IconSet("apply", IconScheme.Colour, "", "Icons.LocateOnDiskToolMedium.png", "Icons.LocateOnDiskToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
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
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }


        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// You may change the name of this method as desired, but be sure to change the
        /// ClickHandler attribute accordingly.
        /// </summary>
        public void Apply()
        {
            if (this.SelectedPresentationImage == null)
                return;

            StandardPresentationImage image = this.SelectedPresentationImage as StandardPresentationImage;
            FileDicomImage dicomImage = image.ImageSop.NativeDicomObject as FileDicomImage;
            
            Platform.OpenFileBrowser(dicomImage.Filename);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using System.IO;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	// This template provides the boiler-plate code for creating a basic tool
	// that performs a single action when its menu item or toolbar button is clicked.

	// Declares a menu action with action ID "apply"
	// TODO: Change the action path hint to your desired menu path, or
	// remove this attribute if you do not want to create a menu item for this tool
	[MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/CreateDynamicTeTool")]

	// Declares a toolbar button action with action ID "apply"
	// TODO: Change the action path hint to your desired toolbar path, or
	// remove this attribute if you do not want to create a toolbar button for this tool
	[ButtonAction("apply", "global-toolbars/ToolbarMyTools/CreateDynamicTeTool")]

	// Specifies tooltip text for the "apply" action
	// TODO: Replace tooltip text
	[Tooltip("apply", "Create Dynamic Te Images")]

	// Specifies icon resources to use for the "apply" action
	// TODO: Replace the icon resource names with your desired icon resources
	[IconSet("apply", IconScheme.Colour, "Icons.CreateDynamicTeToolSmall.png", "Icons.CreateDynamicTeToolMedium.png", "Icons.CreateDynamicTeToolLarge.png")]

	// Specifies that the "apply" action will be handled by a method named "Apply"
	[ClickHandler("apply", "Apply")]

	// Specifies that the enablement of the "apply" action in the user-interface
	// is controlled by observing a boolean property named "Enabled", listening to
	// an event named "EnabledChanged" for changes to this property
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	// Declares this tool as an extension of the ImageViewerToolExtensionPoint extension point.
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]

	public class CreateDynamicTeTool : ImageViewerTool
	{
		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public CreateDynamicTeTool()
		{
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			this.ImageViewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
		}

		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// You may change the name of this method as desired, but be sure to change the
		/// ClickHandler attribute accordingly.
		/// </summary>
		public void Apply()
		{
			IDisplaySet selectedDisplaySet = this.ImageViewer.SelectedImageBox.DisplaySet;
			IDisplaySet t2DisplaySet = new DisplaySet();
			t2DisplaySet.Name = String.Format("{0} - Dynamic TE", selectedDisplaySet.Name);

			double currentSliceLocation = 0.0;

			foreach (IPresentationImage image in selectedDisplaySet.PresentationImages)
			{
				IImageSopProvider imageSopProvider = image as IImageSopProvider;

				if (imageSopProvider == null)
					continue;

				ImageSop imageSop = imageSopProvider.ImageSop;

				if (imageSop.SliceLocation != currentSliceLocation)
				{
					currentSliceLocation = imageSop.SliceLocation;

					DicomFile pdMap = FindMap(imageSop.StudyInstanceUID, imageSop.SliceLocation, "PD");
					pdMap.Load(DicomReadOptions.Default);

					DicomFile t2Map = FindMap(imageSop.StudyInstanceUID, imageSop.SliceLocation, "T2");
					t2Map.Load(DicomReadOptions.Default);

					DicomFile probMap = FindMap(imageSop.StudyInstanceUID, imageSop.SliceLocation, "CHI2PROB");
					probMap.Load(DicomReadOptions.Default);

					DynamicTePresentationImage t2Image = new DynamicTePresentationImage(
						imageSop,
						ConvertToByte((ushort[])pdMap.DataSet[DicomTags.PixelData].Values),
						ConvertToByte((ushort[])t2Map.DataSet[DicomTags.PixelData].Values),
						ConvertToByte((ushort[])probMap.DataSet[DicomTags.PixelData].Values));

					t2Image.SetProbabilityThreshold(20, Color.FromArgb(128, 255, 0, 0));
					t2Image.DynamicTe.Te = 50.0f;
					t2DisplaySet.PresentationImages.Add(t2Image);
				}
			}

			this.ImageViewer.LogicalWorkspace.ImageSets[0].DisplaySets.Add(t2DisplaySet);
		}

		private byte[] ConvertToByte(ushort[] pixelData)
		{
			byte[] newPixelData = new byte[pixelData.Length * 2];
			Buffer.BlockCopy(pixelData, 0, newPixelData, 0, newPixelData.Length);
			return newPixelData;
		}

		private DicomFile FindMap(string studyUID, double sliceLocation, string suffix)
		{
			string directory = String.Format("C:\\dicom_datastore\\T2_MAPS\\{0}", studyUID);
			string[] files = Directory.GetFiles(directory);

			foreach (string file in files)
			{
				string str = String.Format("loc{0:F2}_{1}", sliceLocation, suffix);

				if (file.Contains(str))
					return new DicomFile(file);
			}

			return null;
		}

		/// <summary>
		/// Event handler called when a presentation image is selected.
		/// </summary>
		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			// TODO: add code to handle this event if necessary,
			// or optionally delete this handler if not needed
		}

	}
}

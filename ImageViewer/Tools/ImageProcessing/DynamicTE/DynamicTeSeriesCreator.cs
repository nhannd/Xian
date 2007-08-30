using System;
using System.Drawing;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	static class DynamicTeSeriesCreator
	{
		public static void Create(IImageViewerToolContext viewerContext)
		{
			IDisplaySet selectedDisplaySet = viewerContext.Viewer.SelectedImageBox.DisplaySet;
			IDisplaySet t2DisplaySet = new DisplaySet();
			t2DisplaySet.Name = String.Format("{0} - Dynamic TE", selectedDisplaySet.Name);

			double currentSliceLocation = 0.0;

			BackgroundTask task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
				{
					int i = 0;

					foreach (IPresentationImage image in selectedDisplaySet.PresentationImages)
					{
						IImageSopProvider imageSopProvider = image as IImageSopProvider;

						if (imageSopProvider == null)
							continue;

						ImageSop imageSop = imageSopProvider.ImageSop;

						if (imageSop.SliceLocation != currentSliceLocation)
						{
							currentSliceLocation = imageSop.SliceLocation;

							DynamicTePresentationImage t2Image = CreateT2Image(imageSop);
							t2DisplaySet.PresentationImages.Add(t2Image);
						}

						string message = String.Format("Processing {0} of {1} images", i, selectedDisplaySet.PresentationImages.Count);
						i++;

						BackgroundTaskProgress progress = new BackgroundTaskProgress(i, selectedDisplaySet.PresentationImages.Count, message);
						context.ReportProgress(progress);
					}
				}, false);

			ProgressDialog.Show(task, viewerContext.DesktopWindow, true, ProgressBarStyle.Blocks);

			viewerContext.Viewer.LogicalWorkspace.ImageSets[0].DisplaySets.Add(t2DisplaySet);
		}

		private static DynamicTePresentationImage CreateT2Image(ImageSop imageSop)
		{
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

			t2Image.DynamicTe.Te = 50.0f;
			return t2Image;
		}

		private static byte[] ConvertToByte(ushort[] pixelData)
		{
			byte[] newPixelData = new byte[pixelData.Length * 2];
			Buffer.BlockCopy(pixelData, 0, newPixelData, 0, newPixelData.Length);
			return newPixelData;
		}

		private static DicomFile FindMap(string studyUID, double sliceLocation, string suffix)
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
	}
}

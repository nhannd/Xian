using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class WindowLevelOperator : IMemorable
	{
		private PresentationImage _presentationImage;

		public WindowLevelOperator(PresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			_presentationImage = presentationImage;
		}

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			VOILUTLinear voiLUT = _presentationImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline.VoiLUT as VOILUTLinear;

			if (voiLUT == null)
				throw new InvalidOperationException();

			WindowLevelMemento memento = new WindowLevelMemento();

			memento.WindowWidth = voiLUT.WindowWidth;
			memento.WindowCenter = voiLUT.WindowCenter;

			return memento;
		}

		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			WindowLevelMemento windowLevelMemento = memento as WindowLevelMemento;
			Platform.CheckForInvalidCast(windowLevelMemento, "memento", "WindowLevelMemento");

			GrayscaleLUTPipeline pipeline = _presentationImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline;

			WindowLevelOperator.InstallVOILUTLinear(
				_presentationImage,
				windowLevelMemento.WindowWidth,
				windowLevelMemento.WindowCenter);
		}

		#endregion

		public static void InstallVOILUTLinear(DicomPresentationImage image)
		{
			double windowWidth = image.ImageSop.WindowWidth;
			double windowCenter = image.ImageSop.WindowCenter;

			// Window width has to be at least 1
			if (windowWidth == 0)
				windowWidth = 1;

			WindowLevelOperator.InstallVOILUTLinear(image, windowWidth, windowCenter);
		}

		public static void InstallVOILUTLinear(
			PresentationImage image, 
			double windowWidth,
			double windowCenter)
		{
			GrayscaleLUTPipeline pipeline = image.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline;

			VOILUTLinear voiLUT = null;
			
			// If the pipeline has a VOILUT, check that it's linear
			if (pipeline.VoiLUT != null)
				voiLUT = pipeline.VoiLUT as VOILUTLinear;

			// If the VOILUT on the image is not linear anymore, or
			// if there's no VOILUT on the pipeline to begin with, install a linear one
			if (voiLUT == null)
			{
				IGrayscaleLUT modalityLUT = pipeline.ModalityLUT;
				voiLUT = new VOILUTLinear(modalityLUT.MinOutputValue, modalityLUT.MaxOutputValue);
				pipeline.VoiLUT = voiLUT;
			}

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowCenter;
		}
	}
}
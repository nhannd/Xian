using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class PresentationLutManager : IPresentationLutManager
	{
		private GrayscaleImageGraphic _grayscaleImageGraphic;

		public PresentationLutManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region ILutManager<IPresentationLut,PresentationLutCreationParameters> Members

		public IPresentationLut GetLut()
		{
			return _grayscaleImageGraphic.PresentationLut;
		}

		public void InstallLut(PresentationLutCreationParameters creationParameters)
		{
			_grayscaleImageGraphic.InstallPresentationLut(creationParameters);
		}

		#endregion

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return this.GetLut().GetCreationParametersMemento();
		}

		public void SetMemento(IMemento memento)
		{
			PresentationLutCreationParameters creationParameters = memento as PresentationLutCreationParameters;
			Platform.CheckForInvalidCast(creationParameters, "memento", typeof(PresentationLutCreationParameters).Name);

			if (this.GetLut().TrySetCreationParametersMemento(creationParameters))
				return;

			_grayscaleImageGraphic.InstallPresentationLut(creationParameters);
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class VoiLutManager : IVoiLutManager
	{
		private GrayscaleImageGraphic _grayscaleImageGraphic;

		public VoiLutManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region ILutManager<IVoiLut,VoiLutCreationParameters> Members

		public IVoiLut GetLut()
		{
			return _grayscaleImageGraphic.VoiLut;
		}

		public void InstallLut(VoiLutCreationParameters creationParameters)
		{
			_grayscaleImageGraphic.InstallVoiLut(creationParameters);
		}

		#endregion

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return this.GetLut().GetCreationParametersMemento();
		}

		public void SetMemento(IMemento memento)
		{
			VoiLutCreationParameters creationParameters = memento as VoiLutCreationParameters;
			Platform.CheckForInvalidCast(creationParameters, "memento", typeof(VoiLutCreationParameters).Name);
			_grayscaleImageGraphic.InstallVoiLut(creationParameters);
		}

		#endregion
	}
}

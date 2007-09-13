using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class VoiLutManager : IVoiLutManager
	{
		private GrayscaleImageGraphic _grayscaleImageGraphic;

		public VoiLutManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region IVoiLutManager Members

		public ILut GetLut()
		{
			return _grayscaleImageGraphic.VoiLut;
		}

		public void InstallLut(ILut lut)
		{
			ILut existingLut = GetLut();
			if (existingLut is IGeneratedDataLut)
			{
				//Clear the data in the data lut so it's not hanging around using up memory.
				((IGeneratedDataLut)existingLut).Clear();
			}
			
			_grayscaleImageGraphic.InstallVoiLut(lut);
		}

		#endregion

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return new LutMemento(_grayscaleImageGraphic.VoiLut);
		}

		public void SetMemento(IMemento memento)
		{
			LutMemento lutMemento = memento as LutMemento;
			Platform.CheckForInvalidCast(lutMemento, "memento", typeof(LutMemento).Name);

			if (_grayscaleImageGraphic.VoiLut != lutMemento.OriginatingLut)
				this.InstallLut(lutMemento.OriginatingLut);

			if (lutMemento.InnerMemento != null)
				_grayscaleImageGraphic.VoiLut.SetMemento(lutMemento.InnerMemento);
		}

		#endregion
	}
}

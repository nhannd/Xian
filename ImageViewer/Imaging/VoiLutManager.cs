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

		public IComposableLut GetLut()
		{
			return _grayscaleImageGraphic.VoiLut;
		}

		public void InstallLut(IComposableLut lut)
		{
			IComposableLut existingLut = GetLut();
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
			return new ComposableLutMemento(_grayscaleImageGraphic.VoiLut);
		}

		public void SetMemento(IMemento memento)
		{
			ComposableLutMemento lutMemento = memento as ComposableLutMemento;
			Platform.CheckForInvalidCast(lutMemento, "memento", typeof(ComposableLutMemento).Name);

			if (_grayscaleImageGraphic.VoiLut != lutMemento.OriginatingLut)
				this.InstallLut(lutMemento.OriginatingLut);

			if (lutMemento.InnerMemento != null)
				_grayscaleImageGraphic.VoiLut.SetMemento(lutMemento.InnerMemento);
		}

		#endregion
	}
}

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class PresentationLutManager : IPresentationLutManager
	{
		private GrayscaleImageGraphic _grayscaleImageGraphic;

		public PresentationLutManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region IPresentationLutManager Members

		public IPresentationLut GetLut()
		{
			return _grayscaleImageGraphic.PresentationLut;
		}

		public void InstallLut(string name)
		{
			_grayscaleImageGraphic.InstallPresentationLut(name);
		}

		public void InstallLut(PresentationLutDescriptor descriptor)
		{
			_grayscaleImageGraphic.InstallPresentationLut(descriptor.Name);
		}

		public IEnumerable<PresentationLutDescriptor> AvailablePresentationLuts
		{
			get
			{
				return _grayscaleImageGraphic.AvailablePresentationLuts;
			}
		}

		#endregion

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return new LutMemento(_grayscaleImageGraphic.PresentationLut);
		}

		public void SetMemento(IMemento memento)
		{
			LutMemento lutMemento = memento as LutMemento;
			Platform.CheckForInvalidCast(lutMemento, "memento", typeof(LutMemento).Name);

			if (_grayscaleImageGraphic.PresentationLut != lutMemento.OriginatingLut)
				_grayscaleImageGraphic.InstallPresentationLut(lutMemento.OriginatingLut as IPresentationLut);

			if (lutMemento.InnerMemento != null)
				_grayscaleImageGraphic.PresentationLut.SetMemento(lutMemento.InnerMemento);
		}

		#endregion
	}
}

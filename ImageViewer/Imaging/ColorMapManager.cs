using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class ColorMapManager : IColorMapManager
	{
		private GrayscaleImageGraphic _grayscaleImageGraphic;

		public ColorMapManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region IColorMapManager Members

		public IColorMap GetColorMap()
		{
			return _grayscaleImageGraphic.ColorMap;
		}

		public void InstallColorMap(string name)
		{
			_grayscaleImageGraphic.InstallColorMap(name);
		}

		public void InstallColorMap(ColorMapDescriptor descriptor)
		{
			_grayscaleImageGraphic.InstallColorMap(descriptor.Name);
		}

		public IEnumerable<ColorMapDescriptor> AvailableColorMaps
		{
			get
			{
				return _grayscaleImageGraphic.AvailableColorMaps;
			}
		}

		#endregion

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return new ComposableLutMemento(_grayscaleImageGraphic.ColorMap);
		}

		public void SetMemento(IMemento memento)
		{
			ComposableLutMemento lutMemento = memento as ComposableLutMemento;
			Platform.CheckForInvalidCast(lutMemento, "memento", typeof(ComposableLutMemento).Name);

			if (_grayscaleImageGraphic.ColorMap != lutMemento.OriginatingLut)
				_grayscaleImageGraphic.InstallColorMap(lutMemento.OriginatingLut as IColorMap);

			if (lutMemento.InnerMemento != null)
				_grayscaleImageGraphic.ColorMap.SetMemento(lutMemento.InnerMemento);
		}

		#endregion
	}
}

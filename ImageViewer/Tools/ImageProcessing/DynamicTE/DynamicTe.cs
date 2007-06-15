using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTe : IMemorable
	{
		private ImageGraphic _imageGraphic;
		private double _te;
		private byte[] _protonDensityMap;
		private byte[] _t2Map;

		public DynamicTe(
			ImageGraphic imageGraphic,
			byte[] protonDensityMap,
			byte[] t2Map)
		{
			_imageGraphic = imageGraphic;
			_protonDensityMap = protonDensityMap;
			_t2Map = t2Map;
		}

		internal byte[] ProtonDensityMap
		{
			get { return _protonDensityMap; }
		}

		internal byte[] T2Map
		{
			get { return _t2Map; }
		}

		public double Te
		{
			get { return _te; }
			set
			{
				if (value != _te)
				{
					if (value < 0.0)
						_te = 0.0;
					else
						_te = value;

					CalculatePixelData();
				}
			}
		}

		private unsafe void CalculatePixelData()
		{
			byte[] pixelData = _imageGraphic.PixelData.Raw;
			int sizeInPixels = _imageGraphic.SizeInPixels;

			fixed (byte* pPixelData = pixelData)
			{
				fixed (byte* pProtonDensityMap = _protonDensityMap)
				{
					fixed (byte* pT2Map = _t2Map)
					{
						if (_imageGraphic.IsSigned)
						{
							for (int i = 0; i < sizeInPixels; i++)
							{
								short* pShortPixelData = (short*)pPixelData;
								short* pShortProtonDensityMap = (short*)pProtonDensityMap;
								short* pShortT2Map = (short*)pT2Map;

								short pixelValue = (short)(pShortProtonDensityMap[i] * Math.Exp(-this.Te / pShortT2Map[i]));
								pShortPixelData[i] = pixelValue;
							}
						}
						else
						{
							for (int i = 0; i < sizeInPixels; i++)
							{
								ushort* pShortPixelData = (ushort*)pPixelData;
								ushort* pShortProtonDensityMap = (ushort*)pProtonDensityMap;
								ushort* pShortT2Map = (ushort*)pT2Map;

								ushort pixelValue = (ushort)(pShortProtonDensityMap[i] * Math.Exp(-this.Te / pShortT2Map[i]));
								pShortPixelData[i] = pixelValue;
							}
						}
					}
				}
			}
		}

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return new DynamicTeMemento(this.Te);
		}

		public void SetMemento(IMemento memento)
		{
			DynamicTeMemento teMemento = memento as DynamicTeMemento;

			this.Te = teMemento.Te;
		}

		#endregion
	}
}

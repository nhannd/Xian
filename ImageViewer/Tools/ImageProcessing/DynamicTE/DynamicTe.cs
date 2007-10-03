using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTe : IMemorable
	{
		public class DynamicTeMemento : IMemento
		{
			private double _te;
			private int _threshold;
			private int _opacity;

			public DynamicTeMemento(double te, int threshold, int opacity)
			{
				_te = te;
				_threshold = threshold;
				_opacity = opacity;
			}

			public double Te
			{
				get { return _te; }
			}

			public int Threshold
			{
				get { return _threshold; }
			}

			public int Opacity
			{
				get { return _opacity; }
			}
		}

		private GrayscaleImageGraphic _imageGraphic;
		private ColorImageGraphic _probabilityOverlay;
		private double _te;
		private byte[] _protonDensityMap;
		private byte[] _t2Map;
		private byte[] _probabilityMap;
		private int _threshold;
		private int _opacity;

		public DynamicTe(
			GrayscaleImageGraphic imageGraphic,
			byte[] protonDensityMap,
			byte[] t2Map,
			ColorImageGraphic probabilityOverlay,
			byte[] probabilityMap)
		{
			_imageGraphic = imageGraphic;
			_protonDensityMap = protonDensityMap;
			_t2Map = t2Map;

			_probabilityOverlay = probabilityOverlay;
			_probabilityMap = probabilityMap;
		}

		internal byte[] ProtonDensityMap
		{
			get { return _protonDensityMap; }
		}

		internal byte[] T2Map
		{
			get { return _t2Map; }
		}

		internal byte[] ProbabilityMap
		{
			get { return _probabilityMap; }
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

		public bool ProbabilityMapVisible
		{
			get { return _probabilityOverlay.Visible; }
			set { _probabilityOverlay.Visible = value; }
		}

		public void ApplyProbabilityThreshold(int threshold, int opacity)
		{
			if (!this.ProbabilityMapVisible)
				return;

			_threshold = threshold;
			_opacity = opacity;

			IndexedPixelData probabilityPixelData = new IndexedPixelData(
				_imageGraphic.Rows,
				_imageGraphic.Columns,
				_imageGraphic.BitsPerPixel,
				_imageGraphic.BitsStored,
				_imageGraphic.HighBit,
				_imageGraphic.IsSigned,
				_probabilityMap);

			probabilityPixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (probabilityPixelData.GetPixel(pixelIndex) < threshold)
							_probabilityOverlay.PixelData.SetPixel(pixelIndex, Color.FromArgb(opacity, Color.Red));
						else
							_probabilityOverlay.PixelData.SetPixel(pixelIndex, Color.Empty);
					});
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
			return new DynamicTeMemento(this.Te, _threshold, _opacity);
		}

		public void SetMemento(IMemento memento)
		{
			DynamicTeMemento teMemento = memento as DynamicTeMemento;

			this.Te = teMemento.Te;
			ApplyProbabilityThreshold(teMemento.Threshold, teMemento.Opacity);
			_imageGraphic.Draw();
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A colour <see cref="ImageGraphic"/> with an associated <see cref="ImageSop"/>.
	/// </summary>
	public unsafe class StandardColorImageGraphic : ColorImageGraphic
	{
		#region Private fields

		private bool _converted;
		private ImageSop _imageSop;

		#endregion

		/// <summary>
		/// Instantiates a new instance of <see cref="StandardColorImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		public StandardColorImageGraphic(ImageSop imageSop)
			: base(imageSop.Rows,
				   imageSop.Columns,
				   null)
		{
			if (imageSop.PhotometricInterpretation != PhotometricInterpretation.Rgb &&
				!IsYbr(imageSop.PhotometricInterpretation))
				throw new InvalidOperationException("Image must be RGB or YBR_XXX.");

			_imageSop = imageSop;
		}

		/// <summary>
		/// Gets the associated <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		/// <summary>
		/// Gets the pixel data from the associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// If the photometric interpretation is originally YBR_XXX, accessing this
		/// property for the first time will result in a one time conversion of the
		/// pixel data to RGB.
		/// </remarks>
		protected override byte[] PixelDataRaw
		{
			get
			{
				if (!_converted)
				{
					ConvertToArgb(
						_imageSop.PhotometricInterpretation, 
						_imageSop.PlanarConfiguration,
						_imageSop.PixelData,
						base.PixelDataRaw);

					_converted = true;
				}

				return base.PixelDataRaw;
			}
		}

		private void ConvertToArgb(
			PhotometricInterpretation photometricInterpretation,
			int planarConfiguration,
			byte[] srcPixelData,
			byte[] argbPixelData)
		{
			if (photometricInterpretation == PhotometricInterpretation.Rgb)
			{
				if (planarConfiguration == 0)
					RgbTripletToArgb(srcPixelData, argbPixelData);
				else
					RgbPlanarToArgb(srcPixelData, argbPixelData);
			}
			else
			{
				if (planarConfiguration == 0)
					YbrTripletToArgb(srcPixelData, argbPixelData, photometricInterpretation);
				else
					YbrPlanarToArgb(srcPixelData, argbPixelData, photometricInterpretation);
			}
		}

		#region RGB to ARGB

		private void RgbTripletToArgb(byte[] rgbPixelData, byte[] argbPixelData)
		{
			fixed (byte* pRgbPixelData = rgbPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					for (int i = 0; i < this.SizeInPixels; i++)
					{
						pArgbPixelData[dst] = pRgbPixelData[src + 2];
						pArgbPixelData[dst + 1] = pRgbPixelData[src + 1];
						pArgbPixelData[dst + 2] = pRgbPixelData[src];
						pArgbPixelData[dst + 3] = 0xff;

						src += 3;
						dst += 4;
					}
				}
			}
		}

		private void RgbPlanarToArgb(byte[] rgbPixelData, byte[] argbPixelData)
		{
			fixed (byte* pRgbPixelData = rgbPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					int greenOffset = this.SizeInPixels;
					int blueOffset = this.SizeInPixels * 2;

					for (int i = 0; i < this.SizeInPixels; i++)
					{
						pArgbPixelData[dst] = pRgbPixelData[src + blueOffset];
						pArgbPixelData[dst + 1] = pRgbPixelData[src + greenOffset];
						pArgbPixelData[dst + 2] = pRgbPixelData[src];
						pArgbPixelData[dst + 3] = 0xff;

						src += 1;
						dst += 4;
					}
				}
			}
		}

		#endregion

		#region YBR to ARGB

		private void YbrTripletToArgb(
			byte[] ybrPixelData, 
			byte[] argbPixelData, 
			PhotometricInterpretation photometricInterpretation)
		{
			fixed (byte* pYbrPixelData = ybrPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					YbrToRgb converter = GetYbrToRgbConverter(photometricInterpretation);

					for (int i = 0; i < this.SizeInPixels; i++)
					{
						int rgb = converter(
							pYbrPixelData[src], 
							pYbrPixelData[src + 1], 
							pYbrPixelData[src + 2]);

						pArgbPixelData[dst] = Color.FromArgb(rgb).B;
						pArgbPixelData[dst + 1] = Color.FromArgb(rgb).G;
						pArgbPixelData[dst + 2] = Color.FromArgb(rgb).R;
						pArgbPixelData[dst + 3] = 0xff;

						src += 3;
						dst += 4;
					}
				}
			}
		}

		private void YbrPlanarToArgb(
			byte[] ybrPixelData, 
			byte[] argbPixelData,
			PhotometricInterpretation photometricInterpretation)
		{
			fixed (byte* pYbrPixelData = ybrPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					int bOffset = this.SizeInPixels;
					int rOffset = this.SizeInPixels * 2;

					YbrToRgb converter = GetYbrToRgbConverter(photometricInterpretation);

					for (int i = 0; i < this.SizeInPixels; i++)
					{
						int rgb = converter(
							pYbrPixelData[src], 
							pYbrPixelData[src + bOffset], 
							pYbrPixelData[src + rOffset]);

						pArgbPixelData[dst] = Color.FromArgb(rgb).B;
						pArgbPixelData[dst + 1] = Color.FromArgb(rgb).G;
						pArgbPixelData[dst + 2] = Color.FromArgb(rgb).R;
						pArgbPixelData[dst + 3] = 0xff;

						src += 1;
						dst += 4;
					}
				}
			}
		}

		#endregion

		private YbrToRgb GetYbrToRgbConverter(PhotometricInterpretation photometricInterpretation)
		{
			YbrToRgb converter;

			if (photometricInterpretation == PhotometricInterpretation.YbrFull)
				converter = new YbrToRgb(ColorSpaceConverter.YbrFullToRgb);
			else if (photometricInterpretation == PhotometricInterpretation.YbrFull422)
				converter = new YbrToRgb(ColorSpaceConverter.YbrFull422ToRgb);
			else if (photometricInterpretation == PhotometricInterpretation.YbrIct)
				converter = new YbrToRgb(ColorSpaceConverter.YbrIctToRgb);
			else if (photometricInterpretation == PhotometricInterpretation.YbrPartial422)
				converter = new YbrToRgb(ColorSpaceConverter.YbrPartial422ToRgb);
			else
				converter = new YbrToRgb(ColorSpaceConverter.YbrRctToRgb);

			return converter;
		}

		private bool IsYbr(PhotometricInterpretation photometricInterpretation)
		{
			return photometricInterpretation == PhotometricInterpretation.YbrFull ||
				photometricInterpretation == PhotometricInterpretation.YbrFull422 ||
				photometricInterpretation == PhotometricInterpretation.YbrPartial422 ||
				photometricInterpretation == PhotometricInterpretation.YbrIct ||
				photometricInterpretation == PhotometricInterpretation.YbrRct;
		}
	}
}

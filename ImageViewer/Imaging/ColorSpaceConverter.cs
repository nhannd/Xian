using ClearCanvas.Dicom;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging
{
	public delegate int YbrToRgb(int y, int b, int r);

	/// <summary>
	/// Converts between colour spaces.
	/// </summary>
	public unsafe static class ColorSpaceConverter
	{
		#region Public methods

		/// <summary>
		/// Converts a YBR_FULL value to RGB.
		/// </summary>
		/// <param name="y"></param>
		/// <param name="b"></param>
		/// <param name="r"></param>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrFullToRgb(int y, int b, int r)
		{
			// |r|   | 1.0000  0.0000  1.4020 | | y       |
			// |g| = | 1.0000 -0.3441 -0.7141 | | b - 128 |
			// |b|   | 1.0000  1.7720 -0.0000 | | r - 128 |

			int alpha = 0xff;
			int red = (int)(y + 1.4020 * (r - 128));
			int green = (int)(y - 0.3441 * (b - 128) - 0.7141 * (r - 128));
			int blue = (int)(y + 1.7720 * (b - 128));

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_FULL422 value to RGB.
		/// </summary>
		/// <param name="y"></param>
		/// <param name="b"></param>
		/// <param name="r"></param>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrFull422ToRgb(int y, int b, int r)
		{
			return YbrFullToRgb(y, b, r);
		}

		/// <summary>
		/// Converts a YBR_PARTIAL422 value to RGB.
		/// </summary>
		/// <param name="y"></param>
		/// <param name="b"></param>
		/// <param name="r"></param>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrPartial422ToRgb(int y, int b, int r)
		{
			// |r|   |  1.1644  0.0000  1.5960 | | y - 16  |
			// |g| = |  1.1644 -0.3917 -0.8130 | | b - 128 |
			// |b|   |  1.1644  2.0173  0.0000 | | r - 128 |

			int alpha = 0xff;
			int red = (int)(1.1644 * (y - 16) + 1.5960 * (r - 128));
			int green = (int)(1.1644 * (y - 16) - 0.3917 * (b - 128) - 0.8130 * (r - 128));
			int blue = (int)(1.1644 * (y - 16) + 2.0173 * (b - 128));

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_ICT value to RGB.
		/// </summary>
		/// <param name="y"></param>
		/// <param name="b"></param>
		/// <param name="r"></param>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrIctToRgb(int y, int b, int r)
		{
			// |r|   |  1.00000  0.00000  1.40200 | | y |
			// |g| = |  1.00000 -0.34412 -0.71414 | | b |
			// |b|   |  1.00000  1.77200  0.00000 | | r |

			int alpha = 0xff;
			int red = (int)(y + 1.40200 * r);
			int green = (int)(y - 0.34412 * b - 0.71414 * r);
			int blue = (int)(y + 1.77200 * b);

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_RCT value to RGB.
		/// </summary>
		/// <param name="y"></param>
		/// <param name="b"></param>
		/// <param name="r"></param>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrRctToRgb(int y, int b, int r)
		{
			int alpha = 0xff;
			int green = y - (r + b) / 4;
			int red = r + green;
			int blue = b + green;

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts pixel data of a particular photometric interpretation
		/// to ARGB.
		/// </summary>
		/// <param name="photometricInterpretation"></param>
		/// <param name="planarConfiguration"></param>
		/// <param name="srcPixelData">The input pixel data to be converted.</param>
		/// <param name="argbPixelData">The converted output pixel data in ARGB format.</param>
		public static void ToArgb(
			PhotometricInterpretation photometricInterpretation,
			int planarConfiguration,
			byte[] srcPixelData,
			byte[] argbPixelData)
		{
			int sizeInPixels = argbPixelData.Length / 4;

			if (photometricInterpretation == PhotometricInterpretation.Rgb)
			{
				if (planarConfiguration == 0)
					RgbTripletToArgb(srcPixelData, argbPixelData, sizeInPixels);
				else
					RgbPlanarToArgb(srcPixelData, argbPixelData, sizeInPixels);
			}
			else
			{
				if (planarConfiguration == 0)
					YbrTripletToArgb(srcPixelData, argbPixelData, sizeInPixels, photometricInterpretation);
				else
					YbrPlanarToArgb(srcPixelData, argbPixelData, sizeInPixels, photometricInterpretation);
			}
		}

		#endregion

		#region Private methods

		#region RGB to ARGB

		private static void RgbTripletToArgb(byte[] rgbPixelData, byte[] argbPixelData, int sizeInPixels)
		{
			fixed (byte* pRgbPixelData = rgbPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					for (int i = 0; i < sizeInPixels; i++)
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

		private static void RgbPlanarToArgb(byte[] rgbPixelData, byte[] argbPixelData, int sizeInPixels)
		{
			fixed (byte* pRgbPixelData = rgbPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					int greenOffset = sizeInPixels;
					int blueOffset = sizeInPixels * 2;

					for (int i = 0; i < sizeInPixels; i++)
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

		private static void YbrTripletToArgb(
			byte[] ybrPixelData,
			byte[] argbPixelData,
			int sizeInPixels,
			PhotometricInterpretation photometricInterpretation)
		{
			fixed (byte* pYbrPixelData = ybrPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					YbrToRgb converter = GetYbrToRgbConverter(photometricInterpretation);

					for (int i = 0; i < sizeInPixels; i++)
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

		private static void YbrPlanarToArgb(
			byte[] ybrPixelData,
			byte[] argbPixelData,
			int sizeInPixels, 
			PhotometricInterpretation photometricInterpretation)
		{
			fixed (byte* pYbrPixelData = ybrPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					int bOffset = sizeInPixels;
					int rOffset = sizeInPixels * 2;

					YbrToRgb converter = GetYbrToRgbConverter(photometricInterpretation);

					for (int i = 0; i < sizeInPixels; i++)
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

		private static YbrToRgb GetYbrToRgbConverter(PhotometricInterpretation photometricInterpretation)
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

		#endregion
	}
}

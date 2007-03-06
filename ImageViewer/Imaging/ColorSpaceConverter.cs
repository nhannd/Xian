using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Converts between colour spaces.
	/// </summary>
	public static unsafe class ColorSpaceConverter
	{
		/// <summary>
		/// Converts the specified <see cref="ImageGraphic"/> from
		/// YBR_XXX to RGB.
		/// </summary>
		/// <param name="imageGraphic"></param>
		/// <exception cref="ArgumentException">
		/// Photometric interpretation is not YBR_XXX.
		/// </exception>
		public static void YbrToRgb(ImageGraphic imageGraphic)
		{
			YbrToRgb(
				imageGraphic.PhotometricInterpretation,
				imageGraphic.IsPlanar,
				imageGraphic.Rows,
				imageGraphic.Columns,
				imageGraphic.PixelData.Raw);
		}

		/// <summary>
		/// Converts YBR_XXX pixel data to RGB.
		/// </summary>
		/// <param name="photometricInterpretation"></param>
		/// <param name="isPlanar"></param>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelData"></param>
		public static void YbrToRgb(
			PhotometricInterpretation photometricInterpretation,
			bool isPlanar,
			int rows,
			int columns,
			byte[] pixelData)
		{
			if (photometricInterpretation != PhotometricInterpretation.YbrFull ||
				photometricInterpretation != PhotometricInterpretation.YbrFull422 ||
				photometricInterpretation != PhotometricInterpretation.YbrIct ||
				photometricInterpretation != PhotometricInterpretation.YbrPartial422 ||
				photometricInterpretation != PhotometricInterpretation.YbrRct)
				throw new ArgumentException("Photometric interpretation is not YBR_XXX");

			int imageSizeInPixels = rows * columns;

			if (isPlanar)
			{
				switch (photometricInterpretation)
				{
					case PhotometricInterpretation.YbrFull:
					case PhotometricInterpretation.YbrFull422:
						YbrFullToRgbPlanar(imageSizeInPixels, pixelData);
						break;
					case PhotometricInterpretation.YbrPartial422:
						YbrPartial422ToRgbPlanar(imageSizeInPixels, pixelData);
						break;
					case PhotometricInterpretation.YbrIct:
						YbrIctToRgbPlanar(imageSizeInPixels, pixelData);
						break;
					case PhotometricInterpretation.YbrRct:
						YbrRctToRgbPlanar(imageSizeInPixels, pixelData);
						break;
				}
			}
			else
			{
				switch (photometricInterpretation)
				{
					case PhotometricInterpretation.YbrFull:
					case PhotometricInterpretation.YbrFull422:
						YbrFullToRgbTriplet(imageSizeInPixels, pixelData);
						break;
					case PhotometricInterpretation.YbrPartial422:
						YbrPartial422ToRgbTriplet(imageSizeInPixels, pixelData);
						break;
					case PhotometricInterpretation.YbrIct:
						YbrIctToRgbTriplet(imageSizeInPixels, pixelData);
						break;
					case PhotometricInterpretation.YbrRct:
						YbrRctToRgbTriplet(imageSizeInPixels, pixelData);
						break;
				}
			}
		}

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

		private static void YbrFullToRgbPlanar(int imageSizeInPixels, byte[] pixelData)
		{
			fixed (byte* pBytePixelData = pixelData)
			{
				int y, b, r;
				int bOffset = imageSizeInPixels;
				int rOffset = imageSizeInPixels * 2;

				for (int i = 0; i < imageSizeInPixels; i++)
				{
					y = i;
					b = bOffset + i;
					r = rOffset + i;
					int rgb = YbrFullToRgb(pBytePixelData[y], pBytePixelData[b], pBytePixelData[r]);
					pBytePixelData[y] = (byte)((rgb & 0x00ff0000) >> 16);
					pBytePixelData[b] = (byte)((rgb & 0x0000ff00) >> 8);
					pBytePixelData[r] = (byte)((rgb & 0x000000ff));
				}
			}
		}

		private static void YbrPartial422ToRgbPlanar(int imageSizeInPixels, byte[] pixelData)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private static void YbrIctToRgbPlanar(int imageSizeInPixels, byte[] pixelData)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private static void YbrRctToRgbPlanar(int imageSizeInPixels, byte[] pixelData)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private static void YbrFullToRgbTriplet(int imageSizeInPixels, byte[] pixelData)
		{
			fixed (byte* pBytePixelData = pixelData)
			{
				int y, b, r;

				for (int i = 0; i < imageSizeInPixels; i++)
				{
					y = i * 3;
					b = y + 1;
					r = b + 1;
					int rgb = YbrFullToRgb(pBytePixelData[y], pBytePixelData[b], pBytePixelData[r]);
					pBytePixelData[y] = (byte)((rgb & 0x00ff0000) >> 16);
					pBytePixelData[b] = (byte)((rgb & 0x0000ff00) >> 8);
					pBytePixelData[r] = (byte)((rgb & 0x000000ff));
				}
			}
		}

		private static void YbrPartial422ToRgbTriplet(int imageSizeInPixels, byte[] pixelData)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private static void YbrIctToRgbTriplet(int imageSizeInPixels, byte[] pixelData)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private static void YbrRctToRgbTriplet(int imageSizeInPixels, byte[] pixelData)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}

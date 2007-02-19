using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	public static unsafe class ColorSpaceConverter
	{
		public static void YbrToRgb(ImageGraphic imageGraphic)
		{
			YbrToRgb(
				imageGraphic.PhotometricInterpretation,
				imageGraphic.IsPlanar,
				imageGraphic.Rows,
				imageGraphic.Columns,
				imageGraphic.PixelData.Raw);
		}

		public static void YbrToRgb(
			PhotometricInterpretation photometricInterpretation,
			bool isPlanar,
			int rows,
			int columns,
			byte[] pixelData)
		{
			if (photometricInterpretation == PhotometricInterpretation.Rgb)
				return;

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

		public static int YbrFullToRgb(int y, int b, int r)
		{
			// |r|   | 1.0000  0.0000  1.4020 | | y       |
			// |g| = | 1.0000 -0.3441 -0.7141 | | b - 128 |
			// |b|   | 1.0000  1.7720 -0.0000 | | r - 128 |

			int red = (int)(y + 1.4020 * (r - 128));
			int green = (int)(y - 0.3441 * (b - 128) - 0.7141 * (r - 128));
			int blue = (int)(y + 1.7720 * (b - 128));

			int rgb = (red << 16) + (green << 8) + blue;

			return rgb;
		}

		public static int YbrFull422ToRgb(int y, int b, int r)
		{
			return YbrFullToRgb(y, b, r);
		}

		public static int YbrPartial422ToRgb(int y, int b, int r)
		{
			// |r|   |  1.1644  0.0000  1.5960 | | y - 16  |
			// |g| = |  1.1644 -0.3917 -0.8130 | | b - 128 |
			// |b|   |  1.1644  2.0173  0.0000 | | r - 128 |

			int red = (int)(1.1644 * (y - 16) + 1.5960 * (r - 128));
			int green = (int)(1.1644 * (y - 16) - 0.3917 * (b - 128) - 0.8130 * (r - 128));
			int blue = (int)(1.1644 * (y - 16) + 2.0173 * (b - 128));

			int rgb = (red << 16) + (green << 8) + blue;

			return rgb;
		}

		public static int YbrIctToRgb(int y, int b, int r)
		{
			// |r|   |  1.00000  0.00000  1.40200 | | y |
			// |g| = |  1.00000 -0.34412 -0.71414 | | b |
			// |b|   |  1.00000  1.77200  0.00000 | | r |

			int red = (int)(y + 1.40200 * r);
			int green = (int)(y - 0.34412 * b - 0.71414 * r);
			int blue = (int)(y + 1.77200 * b);

			int rgb = (red << 16) + (green << 8) + blue;

			return rgb;
		}

		public static int YbrRctToRgb(int y, int b, int r)
		{
			int green = y - (r + b) / 4;
			int red = r + green;
			int blue = b + green;

			int rgb = (red << 16) + (green << 8) + blue;

			return rgb;
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

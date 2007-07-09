using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	public delegate int YbrToRgb(int y, int b, int r);

	/// <summary>
	/// Converts between colour spaces.
	/// </summary>
	public static class ColorSpaceConverter
	{

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
	}
}

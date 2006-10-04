using System;
using System.Runtime.InteropServices;

namespace ClearCanvas.ImageViewer.Renderer.GDI
{
	public class Win32
	{
		/// <summary>
		/// Enumeration for the raster operations used in BitBlt.
		/// In C++ these are actually #define. But to use these
		/// constants with C#, a new enumeration type is defined.
		/// </summary>
		public enum TernaryRasterOperations
		{
			SRCCOPY    	=	0x00CC0020, /* dest = source                   */
			SRCPAINT   	=	0x00EE0086, /* dest = source OR dest           */
			SRCAND     	=	0x008800C6, /* dest = source AND dest          */
			SRCINVERT  	=	0x00660046, /* dest = source XOR dest          */
			SRCERASE   	=	0x00440328, /* dest = source AND (NOT dest )   */
			NOTSRCCOPY 	=	0x00330008, /* dest = (NOT source)             */
			NOTSRCERASE	=	0x001100A6, /* dest = (NOT src) AND (NOT dest) */
			MERGECOPY  	=	0x00C000CA, /* dest = (source AND pattern)     */
			MERGEPAINT 	=	0x00BB0226, /* dest = (NOT source) OR dest     */
			PATCOPY    	=	0x00F00021, /* dest = pattern                  */
			PATPAINT   	=	0x00FB0A09, /* dest = DPSnoo                   */
			PATINVERT  	=	0x005A0049, /* dest = pattern XOR dest         */
			DSTINVERT  	=	0x00550009, /* dest = (NOT dest)               */
			BLACKNESS  	=	0x00000042, /* dest = BLACK                    */
			WHITENESS  	=	0x00FF0062, /* dest = WHITE                    */
		};

		/// <summary>
		/// CreateCompatibleDC
		/// </summary>
		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		/// <summary>
		/// DeleteDC
		/// </summary>
		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
		public static extern bool DeleteDC(IntPtr hdc);

		/// <summary>
		/// SelectObject
		/// </summary>
		[DllImport("gdi32.dll", ExactSpelling=true)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		/// <summary>
		/// DeleteObject
		/// </summary>
		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
		public static extern bool DeleteObject(IntPtr hObject);

		/// <summary>
		/// CreateCompatibleBitmap
		/// </summary>
		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hObject, int width, int height);

		/// <summary>
		/// BitBlt
		/// </summary>
		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
		public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
	}
}

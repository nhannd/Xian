using System;

namespace ClearCanvas.Dicom.Iod
{
	[Flags]
	public enum ShutterShape
	{
		None,
		Circular = 1,
		Rectangular = 2,
		Polygonal = 4,
		Bitmap = 8
	}
}

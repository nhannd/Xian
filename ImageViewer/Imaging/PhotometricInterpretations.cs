using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workstation.Model.Imaging
{
	public enum PhotometricInterpretations
	{
		Unknown			= 0,
		Monochrome1		= 1,
		Monochrome2		= 2,
		PaletteColor	= 3,
		Rgb				= 4,
		YbrFull			= 5,
		YbrFull422		= 6,
		YbrPartial422	= 7,
		YbrIct			= 8,
		YbrRct			= 9
	}
}

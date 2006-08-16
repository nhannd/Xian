using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public enum PhotometricInterpretation
    {
        Unknown = 0,
        Monochrome1,
        Monochrome2,
        PaletteColor,
        Rgb,
        Hsv,
        Argb,
        Cmyk,
        YbrFull,
        YbrFull422,
        YbrPartial422,
        YbrPartial420,
        YbrIct,
        YbrRct
    }
}

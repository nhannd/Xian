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

    public class PhotometricInterpretationHelper
    {
        static private Dictionary<PhotometricInterpretation, string> _dictionaryPhotometricInterpretation;
        static PhotometricInterpretationHelper()
        {
            _dictionaryPhotometricInterpretation = new Dictionary<PhotometricInterpretation, string>();
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Unknown, "UNKNOWN");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Argb, "ARGB");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Cmyk, "CMYK");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Hsv, "HSV");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Monochrome1, "MONOCHROME1");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Monochrome2, "MONOCHROME2");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.PaletteColor, "PALETTE_COLOR");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Rgb, "RGB");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrFull, "YBR_FULL");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrFull422, "YBR_FULL_422");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrIct, "YBR_ICT");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrPartial420, "YBR_PARTIAL_420");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrPartial422, "YBR_PARTIAL_422");
            _dictionaryPhotometricInterpretation.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrRct, "YBR_RCT");
        }

        public static string GetString(PhotometricInterpretation pi)
        {
            if (_dictionaryPhotometricInterpretation.ContainsKey(pi))
                return _dictionaryPhotometricInterpretation[pi];
            else
                return null;
        }
    }
}

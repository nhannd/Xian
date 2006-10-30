using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
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
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.Unknown, "UNKNOWN");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.Argb, "ARGB");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.Cmyk, "CMYK");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.Hsv, "HSV");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.Monochrome1, "MONOCHROME1");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.Monochrome2, "MONOCHROME2");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.PaletteColor, "PALETTE_COLOR");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.Rgb, "RGB");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.YbrFull, "YBR_FULL");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.YbrFull422, "YBR_FULL_422");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.YbrIct, "YBR_ICT");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.YbrPartial420, "YBR_PARTIAL_420");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.YbrPartial422, "YBR_PARTIAL_422");
            _dictionaryPhotometricInterpretation.Add(PhotometricInterpretation.YbrRct, "YBR_RCT");
        }

        public static string GetString(PhotometricInterpretation pi)
        {
            if (_dictionaryPhotometricInterpretation.ContainsKey(pi))
                return _dictionaryPhotometricInterpretation[pi];
            else
                return null;
        }

		public static PhotometricInterpretation FromString(string photometricInterpretation)
		{
			foreach (KeyValuePair<PhotometricInterpretation, string> pair in _dictionaryPhotometricInterpretation)
			{
				if (pair.Key == PhotometricInterpretation.Unknown)
					continue;

				if (photometricInterpretation == pair.Value)
					return pair.Key;
			}

			return PhotometricInterpretation.Unknown;
		}
    }
}

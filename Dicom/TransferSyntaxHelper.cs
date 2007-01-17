using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom
{
    public class TransferSyntaxHelper
    {
        static private Dictionary<E_TransferSyntax, string> _dictionaryTransferSyntax;

        static TransferSyntaxHelper()
        {
            _dictionaryTransferSyntax = new Dictionary<E_TransferSyntax, string>();
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_LittleEndianImplicit, "1.2.840.10008.1.2");
            //_dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_BigEndianImplicit, "");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_LittleEndianExplicit, "1.2.840.10008.1.2.1");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_BigEndianExplicit, "1.2.840.10008.1.2.2");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess1TransferSyntax, "1.2.840.10008.1.2.4.50");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess2_4TransferSyntax, "1.2.840.10008.1.2.4.51");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess3_5TransferSyntax, "1.2.840.10008.1.2.4.52");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess6_8TransferSyntax, "1.2.840.10008.1.2.4.53");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess7_9TransferSyntax, "1.2.840.10008.1.2.4.54");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess10_12TransferSyntax, "1.2.840.10008.1.2.4.55");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess11_13TransferSyntax, "1.2.840.10008.1.2.4.56");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess14TransferSyntax, "1.2.840.10008.1.2.4.57");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess15TransferSyntax, "1.2.840.10008.1.2.4.58");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess16_18TransferSyntax, "1.2.840.10008.1.2.4.59");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess17_19TransferSyntax, "1.2.840.10008.1.2.4.60");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess20_22TransferSyntax, "1.2.840.10008.1.2.4.61");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess21_23TransferSyntax, "1.2.840.10008.1.2.4.62");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess24_26TransferSyntax, "1.2.840.10008.1.2.4.63");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess25_27TransferSyntax, "1.2.840.10008.1.2.4.64");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess28TransferSyntax, "1.2.840.10008.1.2.4.65");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess29TransferSyntax, "1.2.840.10008.1.2.4.66");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGProcess14SV1TransferSyntax, "1.2.840.10008.1.2.4.70");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_RLELossless, "1.2.840.10008.1.2.5");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGLSLossless, "1.2.840.10008.1.2.4.80");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEGLSLossy, "1.2.840.10008.1.2.4.81");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_DeflatedLittleEndianExplicit, "1.2.840.10008.1.2.1.99");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEG2000LosslessOnly, "1.2.840.10008.1.2.4.90");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEG2000, "1.2.840.10008.1.2.4.91");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_MPEG2MainProfileAtMainLevel, "1.2.840.10008.1.2.4.100");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEG2000MulticomponentLosslessOnly, "1.2.840.10008.1.2.4.92");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_JPEG2000Multicomponent, "1.2.840.10008.1.2.4.92");
            _dictionaryTransferSyntax.Add(E_TransferSyntax.EXS_Unknown, "");
        }

        public static string GetString(E_TransferSyntax transferSyntax)
        {
            if (_dictionaryTransferSyntax.ContainsKey(transferSyntax))
                return _dictionaryTransferSyntax[transferSyntax];
            else
                return null;
        }

        public static E_TransferSyntax FromString(string transferSyntax)
        {
            foreach (KeyValuePair<E_TransferSyntax, string> pair in _dictionaryTransferSyntax)
            {
                if (pair.Key == E_TransferSyntax.EXS_Unknown)
                    continue;

                if (transferSyntax == pair.Value)
                    return pair.Key;
            }

            return E_TransferSyntax.EXS_Unknown;
        }
    }
}

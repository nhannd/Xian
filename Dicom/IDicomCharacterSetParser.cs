using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Public interface used to define a parser to convert between raw bytes
    /// and Unicode.
    /// </summary>
    public interface IDicomCharacterSetParser
    {
        byte[] Encode(string unicodeString, string specificCharacterSet);
        string Decode(byte[] repertoireStringAsRaw, string specificCharacterSet);
        string EncodeAsIsomorphicString(string unicodeString, string specificCharacterSet);
        string DecodeFromIsomorphicString(string repertoireStringAsUnicode, string specificCharacterSet);
        string ConvertRawToIsomorphicString(byte[] repertoireStringAsRaw);
        bool IsVRRelevant(string vr);
    }
}

#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

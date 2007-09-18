using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    internal interface IDicomDictionary
    {
        bool Contains(TagName tagName);
        bool Contains(DicomTagPath path);
        DictionaryEntry GetColumn(TagName tagName);
        DictionaryEntry GetColumn(DicomTagPath path);
    }
}

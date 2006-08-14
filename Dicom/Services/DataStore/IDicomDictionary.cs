using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDicomDictionary
    {
        bool Contains(TagName tagName);
        bool Contains(Path path);
        DictionaryEntry GetColumn(TagName tagName);
        DictionaryEntry GetColumn(Path path);
    }
}

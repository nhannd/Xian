using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom
{
    public interface IDicomCharacterSetParser
    {
        byte[] Encode(String value, String specificCharacterSet);
        String Decode(byte[] value, String specificCharacterSet);
    }
}

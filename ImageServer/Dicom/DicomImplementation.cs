using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom
{

    public static class DicomImplementation
    {
        public static DicomUid ClassUID = new DicomUid("1.2.840.999999.1", "Implementation Class UID", UidType.Unknown);
        public static string Version = "Dicom 0.1";
    }

}

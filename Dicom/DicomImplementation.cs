using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    public static class DicomImplementation
    {
        public static DicomUid ClassUID = new DicomUid("1.3.6.1.4.1.25403.1.1.1", "Implementation Class UID", UidType.Unknown);
        public static string Version = "Dicom 0.1";
        public static IDicomCharacterSetParser CharacterParser = new SpecificCharacterSetParser();
    }

}

using System;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Represents an exception thrown when an attempt is made to set the dicom text value
    /// which character(s) which is not covered by the specific character set of the Dicom attribute collection 
    /// which the attribute is part of.
    /// </summary>
    public class DicomCharacterSetException : Exception
    {
        public DicomCharacterSetException(uint tag, string characterSets, string offendedValue, string message)
            : base(message)
        {
            DicomTag = tag;
            SpecificCharacterSets = characterSets;
            OffendedValue = offendedValue;
        }

        public uint DicomTag { get; private set; }
        public string SpecificCharacterSets { get; private set; }
        public string OffendedValue { get; private set; }
    }
}
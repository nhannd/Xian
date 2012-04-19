using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public static class StoreEntryExtensions
    {
        public static DicomAttributeCollection ToDicomAttributeCollection(this StudyEntry studyEntry)
        {
            var collection = new DicomAttributeCollection {ValidateVrLengths = false, IgnoreOutOfRangeTags = true, ValidateVrValues = false};

            if (studyEntry.Study != null)
                collection.SaveDicomFields(studyEntry.Study);
            if (studyEntry.Data != null)
                collection.SaveDicomFields(studyEntry.Data);

            return collection;
        }
    }
}

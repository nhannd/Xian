using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public static class StoreEntryExtensions
    {
        public static DicomAttributeCollection ToDicomAttributeCollection(this StudyEntry studyEntry)
        {
            var collection = new DicomAttributeCollection {ValidateVrLengths = false, IgnoreOutOfRangeTags = true, ValidateVrValues = false};

            var tag = new DicomTag(DicomTags.BitsAllocated, "Bits Allocated", "BitsAllocated", DicomVr.ISvr, true, 1, Int32.MaxValue, false);
            collection[tag].SetEmptyValue();
            var bitsAllocated = collection[tag];
            
            tag = new DicomTag(DicomTags.BitsStored, "Bits Stored", "BitsStored", DicomVr.ISvr, true, 1, Int32.MaxValue, false);
            collection[tag].SetEmptyValue();
            var bitsStored = collection[tag];

            if (studyEntry.Study != null)
            {
                collection.SaveDicomFields(studyEntry.Study);
            }
            if (studyEntry.Data != null)
            {
                collection.SaveDicomFields(studyEntry.Data);

                if (studyEntry.Data.BitsAllocatedInStudy != null)
                {
                    if (studyEntry.Data.BitsAllocatedInStudy.Length == 0)
                    {
                        bitsAllocated.SetNullValue();
                    }
                    else
                    {
                        foreach (var value in studyEntry.Data.BitsAllocatedInStudy)
                            bitsAllocated.AppendInt32(value);
                    }
                }

                if (studyEntry.Data.BitsStoredInStudy != null)
                {
                    if (studyEntry.Data.BitsStoredInStudy.Length == 0)
                    {
                        bitsStored.SetNullValue();
                    }
                    else
                    {
                        foreach (var value in studyEntry.Data.BitsStoredInStudy)
                            bitsStored.AppendInt32(value);
                    }
                }
            }

            return collection;
        }
    }
}

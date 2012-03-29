using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal static class Utilities
    {
        public static DicomAttribute GetAttribute(this DicomTagPath path, IDicomAttributeProvider attributes)
        {
            return GetAttribute(path, attributes, false);
        }

        public static DicomAttribute GetAttribute(this DicomTagPath path, IDicomAttributeProvider attributes, bool create)
        {
            DicomAttribute attribute = null;
            var count = path.TagsInPath.Count;
            for (int i = 0; i < count; i++)
            {
                attribute = attributes[path.TagsInPath[i]];
                if (i == count - 1)
                    break;
                
                if (attribute.IsEmpty && !create)
                    return null;

                var sequenceItems = attribute.Values as DicomSequenceItem[];
                if (sequenceItems == null)
                    return null;

                attributes = sequenceItems[0];
            }

            return attribute;
        }
    }
}

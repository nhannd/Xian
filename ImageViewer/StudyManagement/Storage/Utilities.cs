using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    internal static class Utilities
    {
        public static DicomAttribute GetAttribute(this DicomTagPath path, IDicomAttributeProvider attributes)
        {
            return GetAttribute(path, attributes, false);
        }

        public static DicomAttribute GetAttribute(this DicomTagPath path, IDicomAttributeProvider attributes, bool create)
        {
            DicomAttribute attribute;
            var tags = new Queue<DicomTag>(path.TagsInPath);

            do
            {
                var tag = tags.Dequeue();
                attribute = attributes[tag];
                if (tags.Count == 0)
                    break;

                var sequenceItems = attribute.Values as DicomSequenceItem[];
                if (sequenceItems == null || sequenceItems.Length == 0)
                {
                    if (!create)
                        return null;

                    attribute.AddSequenceItem(new DicomSequenceItem());
                    sequenceItems = (DicomSequenceItem[]) attribute.Values;
                }

                attributes = sequenceItems[0];
            } while (tags.Count > 0);

            if (attribute.IsEmpty && create)
                attribute.SetNullValue();

            return attribute.IsEmpty ? null : attribute;
        }
    }
}

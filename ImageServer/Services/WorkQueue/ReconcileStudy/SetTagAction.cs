using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{

    /// <summary>
    /// Helper class to represent the content specified in a "SetTag" xml node.
    /// </summary>
    class UpdateSpecification
    {
        private List<DicomTag> _parentTags = null;
        private DicomTag _targetTag = null;
        private string _value = null;

        public UpdateSpecification(List<DicomTag> parentTags, DicomTag tag, String value)
        {
            _parentTags = parentTags;
            _targetTag = tag;
            _value = value;
        }

        public List<DicomTag> ParentTags
        {
            get { return _parentTags; }
            set { _parentTags = value; }
        }

        public DicomTag TargetTag
        {
            get { return _targetTag; }
            set { _targetTag = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }


    }

    /// <summary>
    /// Represents a "SetTag" action within a "UpdateImages" command
    /// </summary>
    class SetTagAction : IDicomFileUpdateCommandAction
    {
        private UpdateSpecification specifications;

        public UpdateSpecification Specifications
        {
            get { return specifications; }
            set { specifications = value; }
        }

        #region IDicomFileUpdateCommandAction Members

        public void Apply(DicomFile file)
        {
            Platform.Log(LogLevel.Info, "Updating dicom file...");

            DicomAttribute attribute = FindAttribute(file.DataSet);
            if (attribute != null)
            {
                Platform.Log(LogLevel.Info, "Updating {0} to '{1}'", attribute.Tag, Specifications.Value);

                if (Specifications.Value == null)
                    attribute.SetNullValue();
                else
                    attribute.SetStringValue(Specifications.Value);
            }
        }

        private DicomAttribute FindAttribute(DicomAttributeCollection collection)
        {
            if (collection == null)
                return null;

            if (Specifications.ParentTags != null)
            {
                foreach (DicomTag tag in Specifications.ParentTags)
                {
                    DicomAttribute sq = collection[tag] as DicomAttributeSQ;
                    if (sq == null)
                    {
                        throw new Exception(String.Format("Invalid tag value: {0}({1}) is not a SQ VR", tag, tag.Name));
                    }
                    if (sq.IsEmpty)
                    {
                        DicomSequenceItem item = new DicomSequenceItem();
                        sq.AddSequenceItem(item);
                    }

                    DicomSequenceItem[] items = sq.Values as DicomSequenceItem[];
                    Platform.CheckForNullReference(items, "items");
                    collection = items[0];
                }
            }

            return collection[Specifications.TargetTag];
        }

        #endregion
    }
}

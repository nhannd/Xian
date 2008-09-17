using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Defines the interface for actions which are defined based on <see cref="TagUpdateSpecification"/>
    /// </summary>
    /// <remark>
    /// A <see cref="ITagBasedUpdateAction"> action consist of <see cref="TagUpdateSpecification"/>
    /// describing the tags that need to be updated and what the new values are.
    /// </remark>
    public interface ITagBasedUpdateAction
    {
        /// <summary>
        /// Gets the specifications which define the action
        /// </summary>
        List<TagUpdateSpecification> Specifications { get; }
    }

    /// <summary>
    /// Tag level update specifications.
    /// </summary>
    /// <remarks>
    /// <see cref="TagUpdateSpecification"/> specificies the tag that needs to be updated and its new value.
    /// </remarks>
    public class TagUpdateSpecification
    {
        #region Private Members
        private readonly List<DicomTag> _parentTags = null;
        private readonly DicomTag _targetTag = null;
        private readonly string _value = null;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="TagUpdateSpecification"/> for specified Dicom tag and value.
        /// </summary>
        /// <param name="parentTags">Ascendant tag in the Dicom image</param>
        /// <param name="tag">The tag that needs to be updated</param>
        /// <param name="value">The new value for the tag</param>
        public TagUpdateSpecification(List<DicomTag> parentTags, DicomTag tag, String value)
        {
            _parentTags = parentTags;
            _targetTag = tag;
            _value = value;
        }
        #endregion

        /// <summary>
        /// Gets the parents of the <see cref="TargetTag"/>
        /// </summary>
        public List<DicomTag> ParentTags
        {
            get { return _parentTags; }
        }

        /// <summary>
        /// Gets the Dicom tag whose value will be updated.
        /// </summary>
        public DicomTag TargetTag
        {
            get { return _targetTag; }
        }

        /// <summary>
        /// Gets the value to be set according to the specications
        /// </summary>
        public string Value
        {
            get { return _value; }
        }


    }

    /// <summary>
    /// Represents a "SetTag" action that can be applied to a Dicom file.
    /// </summary>
    /// <remarks>
    /// </remarks>
    class SetTagAction : IDicomFileUpdateCommandAction, ITagBasedUpdateAction
    {
        #region Private Members
        private readonly TagUpdateSpecification _specifications;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="SetTagAction"/> based on the given specifications.
        /// </summary>
        /// <param name="specifications"></param>
        public SetTagAction (TagUpdateSpecification specifications)
        {
            _specifications = specifications;
        }
        #endregion

        #region IDicomFileUpdateCommandAction Members

        public void Apply(DicomFile file)
        {
            Platform.Log(LogLevel.Debug, "Updating dicom file...");

            DicomAttribute attribute = FindAttribute(file.DataSet);
            if (attribute != null)
            {
                Platform.Log(LogLevel.Debug, "Updating {0} to '{1}'", attribute.Tag, _specifications.Value);

                if (_specifications.Value == null)
                    attribute.SetNullValue();
                else
                    attribute.SetStringValue(_specifications.Value);
            }
        }

        private DicomAttribute FindAttribute(DicomAttributeCollection collection)
        {
            if (collection == null)
                return null;

            if (_specifications.ParentTags != null)
            {
                foreach (DicomTag tag in _specifications.ParentTags)
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

            return collection[_specifications.TargetTag];
        }

        #endregion
    
        #region ITagBasedUpdateAction Members

        public List<TagUpdateSpecification>  Specifications
        {
	        get {
	            List<TagUpdateSpecification> list = new List<TagUpdateSpecification>();
	            list.Add(_specifications);
	            return list;
            }
        }

        #endregion
}
}

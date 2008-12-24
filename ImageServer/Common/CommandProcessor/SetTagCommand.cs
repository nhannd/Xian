using System;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Helpers;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    /// <summary>
    /// Represents a command that can be executed on an <see cref="DicomFile"/>
    /// </summary>
    /// <remarks>
    /// This class is serializable.
    /// </remarks>
    [XmlRoot("SetTag")]
    public class SetTagCommand : BaseImageLevelUpdateCommand, IUpdateImageTagCommand
    {
        #region Private Fields
        private ImageLevelUpdateEntry _updateEntry = new ImageLevelUpdateEntry();
        #endregion

        #region Constructors
        public SetTagCommand()
            : base("SetTag")
        {
            Description = "Update Dicom Tag";
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="ImageLevelUpdateEntry"/> for this command.
        /// </summary>
        [XmlIgnore]
        public ImageLevelUpdateEntry UpdateEntry
        {
            get { return _updateEntry; }
            set { _updateEntry = value; }
        }

        /// <summary>
        /// Gets the name of the Dicom tag affected by this command.
        /// **** For XML serialization purpose. ****
        /// </summary>
        [XmlAttribute(AttributeName = "TagName")]
        public string TagName
        {
            get { return _updateEntry.TagPath.Tag.Name; }
            set
            {
                // NO-OP 
            }
        }

        /// <summary>
        /// Gets or sets the Dicom tag value to be used by this command when updating the dicom file.
        /// </summary>
        [XmlAttribute(AttributeName = "Value")]
        public string Value
        {
            get
            {
                if (_updateEntry == null)
                    return null;

                return _updateEntry.Value != null ? _updateEntry.Value.ToString() : null;
            }
            set
            {
                _updateEntry.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets 
        /// </summary>
        [XmlAttribute(AttributeName = "TagPath")]
        public string TagPath
        {
            get { return _updateEntry.TagPath.HexString(); }
            set
            {
                DicomTagPathConverter converter = new DicomTagPathConverter();
                _updateEntry.TagPath = (DicomTagPath)converter.ConvertFromString(value);
            }
        }

        #endregion

        #region IImageLevelCommand Members

        public override bool Apply(DicomFile file)
        {
            if (_updateEntry != null)
            {
                DicomAttribute attr = FindAttribute(file.DataSet, UpdateEntry);
                if (attr != null)
                    attr.SetStringValue(UpdateEntry.GetStringValue());
            }

            return true;
        }

        public override string ToString()
        {
            return String.Format("Set {0}={1}", UpdateEntry.TagPath.Tag, UpdateEntry.Value);
        }
        #endregion

        #region IImageLevelCommand Members

        #endregion



        protected DicomAttribute FindAttribute(DicomAttributeCollection collection, ImageLevelUpdateEntry entry)
        {
            if (collection == null)
                return null;

            if (entry.TagPath.Parents != null)
            {
                foreach (DicomTag tag in entry.TagPath.Parents)
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

            return collection[entry.TagPath.Tag];
        }
    }
}

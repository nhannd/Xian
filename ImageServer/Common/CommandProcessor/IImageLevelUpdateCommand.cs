using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    /// <summary>
    /// Defines the interface of an image-level update command
    /// </summary>
    public interface IImageLevelUpdateCommand
    {
        /// <summary>
        /// Applies changes to specified <see cref="DicomFile"/>
        /// </summary>
        /// <param name="file"></param>
        void Apply(DicomFile file);
    }

    /// <summary>
    /// Defines the interface of an image-level update command which modifies a tag in a Dicom file.
    /// </summary>
    public interface IUpdateImageTagCommand:IImageLevelUpdateCommand
    {
        /// <summary>
        /// Gets the <see cref="ImageLevelUpdateEntry"/> associated with the command
        /// </summary>
        ImageLevelUpdateEntry UpdateEntry { get; }
    }

    /// <summary>
    /// Encapsulates the tag update specification
    /// </summary>
    public class ImageLevelUpdateEntry
    {
        #region Private members
        private DicomTag _tag;
        private List<DicomTag> _parentTags;
        private object _value;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the tag to be updated
        /// </summary>
        public DicomTag Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// Gets or sets the parents of the tag that is updated.
        /// </summary>
        public List<DicomTag> ParentTags
        {
            get { return _parentTags; }
            set { _parentTags = value; }
        }

        /// <summary>
        /// Gets or sets the value of the tag to be updated
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets the value of the tag as a string 
        /// </summary>
        /// <returns></returns>
        public string GetStringValue()
        {
            if (_value == null)
                return String.Empty;
            else
                return _value.ToString();
        }

        #endregion

    }

    
}

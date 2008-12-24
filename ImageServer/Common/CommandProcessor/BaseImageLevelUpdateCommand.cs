using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Helpers;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{

    /// <summary>
    /// Defines the interface of an <see cref="BaseImageLevelUpdateCommand"/> which modifies a tag in a Dicom file.
    /// </summary>
    public interface IUpdateImageTagCommand
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
        private DicomTagPath _tagPath = new DicomTagPath();
        private object _value;
        #endregion

        #region Public Properties

        public DicomTagPath TagPath
        {
            get { return _tagPath; }
            set { _tagPath = value; }
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
                return null;
            else
                return _value.ToString();
        }

        #endregion

    }


    public abstract class BaseImageLevelUpdateCommand : ServerCommand
    {
        private DicomFile _file;
        private string _name;


        public BaseImageLevelUpdateCommand()
            : base("ImageLevelUpdateCommand", true)
        {
        }

        public BaseImageLevelUpdateCommand(string name)
            : base("ImageLevelUpdateCommand", true)
        {
            _name = name;
        }

        #region IActionItem<DicomFile> Members

        public abstract bool Apply(DicomFile file);

        #endregion

        #region IImageLevelUpdateOperation Members

        [XmlIgnore]
        public string CommandName
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlIgnore]
        public DicomFile File
        {
            set { _file = value; }
        }


        #endregion

        protected override void OnExecute()
        {
            if (_file != null)
                Apply(_file);
        }

        protected override void OnUndo()
        {
            // NO-OP
        }

        #region IImageLevelUpdateCommand Members

       
        #endregion
    }


    
}

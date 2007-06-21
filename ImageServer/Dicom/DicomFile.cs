using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Offis;

namespace ClearCanvas.ImageServer.Dicom
{
    /// <summary>
    /// Class representing a DICOM Part 10 Format File.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class represents a DICOM Part 10 format file.  The class inherits off an AbstractMessage class.  The class contains
    /// <see cref="AttributeCollection"/> instances for the Meta Info (group 0x0002 attributes) and Data Set. 
    /// </para>
    /// </remarks>
    public class DicomFile : AbstractMessage
    {
        #region Private Members

        private String _filename = null;

        /// <summary>
        /// Offis DcmFileFormat instance
        /// </summary>
        private DcmFileFormat _fileFormat = null;

        #endregion

        #region Constructors
        public DicomFile(String filename, AttributeCollection metaInfo, AttributeCollection dataSet)
        {
            base._metaInfo = metaInfo;
            base._dataSet = dataSet;
            _filename = filename;
        }

        public DicomFile(String filename)
        {
            base._metaInfo = new AttributeCollection();
            base._dataSet = new AttributeCollection();

            _filename = filename;
        }
        #endregion

        #region Properties

        /// <summary>
        /// The filename of the file.
        /// </summary>
        /// <remarks>
        /// This property sets/gets the filename associated with the file.
        /// </remarks>
        public String Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        /// <summary>
        /// The SOP Class of the file.
        /// </summary>
        /// <remarks>
        /// This property returns a <see cref="SopClass"/> object for the sop class
        /// encoded in the tag Media Storage SOP Class UID (0002,0002).
        /// </remarks>
        public SopClass SopClass
        {
            get
            {
                String sopClassUid = base.MetaInfo[DicomTags.MediaStorageSOPClassUID].ToString();

                return SopClass.GetSopClass(sopClassUid);
            }
        }

        /// <summary>
        /// The transfer syntax the file is encoded in.
        /// </summary>
        /// <remarks>
        /// This property returns a TransferSyntax object for the transfer syntax encoded 
        /// in the tag Transfer Syntax UID (0002,0010).
        /// </remarks>
        public TransferSyntax TransferSyntax
        {
            get
            {
                String transferSyntaxUid = base._metaInfo[DicomTags.TransferSyntaxUID];

                return TransferSyntax.GetTransferSyntax(transferSyntaxUid);
            }
            set
            {
                base._metaInfo[DicomTags.TransferSyntaxUID].SetStringValue(value.Uid);
            }
        }

        #endregion

        #region Public Methods

        public bool Load()
        {
            if (_fileFormat == null)
                _fileFormat = new DcmFileFormat();

            if ((base._metaInfo.OffisDataset == null)
                && base._dataSet.OffisDataset == null)
            {
                OFCondition status = _fileFormat.loadFile(_filename, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_noChange, 64, E_FileReadMode.ERM_autoDetect);
                OffisHelper.CheckReturnValue(status, _filename);

                base._dataSet.OffisDataset = new OffisDcmItem(_fileFormat.getDataset(),_fileFormat);
                base._metaInfo.OffisDataset = new OffisDcmItem(_fileFormat.getMetaInfo(), _fileFormat);
            }
	
            return true;
        }

        public bool Write()
        {
            if (_fileFormat == null)
                _fileFormat = new DcmFileFormat();

            if ((base._metaInfo.OffisDataset == null)
               && base._dataSet.OffisDataset == null)
            {
                base._dataSet.OffisDataset = new OffisDcmItem(_fileFormat.getDataset(), _fileFormat);
                base._metaInfo.OffisDataset = new OffisDcmItem(_fileFormat.getMetaInfo(), _fileFormat);
            }

            base._metaInfo.FlushDirtyAttributes();
            base._dataSet.FlushDirtyAttributes();

            OFCondition status = _fileFormat.saveFile(_filename, OffisHelper.ConvertTransferSyntax(this.TransferSyntax));
            OffisHelper.CheckReturnValue(status, _filename);

            return true;
        }

        #endregion

    }
}

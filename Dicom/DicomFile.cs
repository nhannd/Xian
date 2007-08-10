using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Class representing a DICOM Part 10 Format File.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class represents a DICOM Part 10 format file.  The class inherits off an AbstractMessage class.  The class contains
    /// <see cref="DicomAttributeCollection"/> instances for the Meta Info (group 0x0002 attributes) and Data Set. 
    /// </para>
    /// </remarks>
    public class DicomFile : DicomMessageBase
    {
        #region Private Members

        private String _filename = null;

        #endregion

        #region Constructors
        public DicomFile(String filename, DicomAttributeCollection metaInfo, DicomAttributeCollection dataSet)
        {
            base._metaInfo = metaInfo;
            base._dataSet = dataSet;
            _filename = filename;
        }

        public DicomFile(String filename)
        {
            base._metaInfo = new DicomAttributeCollection(0x00020000, 0x0002FFFF);
            base._dataSet = new DicomAttributeCollection(0x00040000, 0xFFFFFFFF);

            _filename = filename;
        }
        public DicomFile(DicomMessage msg, String filename)
        {
            base._metaInfo = new DicomAttributeCollection(0x00020000,0x0002FFFF);
            base._dataSet = msg.DataSet;

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
                String sopClassUid = base.MetaInfo[DicomTags.MediaStorageSOPClassUID].GetString(0,"");

                SopClass sop = SopClass.GetSopClass(sopClassUid);

                if (sop == null)
                    sop = new SopClass("Unknown Sop Class", sopClassUid, false);

                return sop;
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
                base._metaInfo[DicomTags.TransferSyntaxUID].SetStringValue(value.UidString);
            }
        }

        #endregion

        #region Meta Info Properties
        /// <summary>
        /// Uniquiely identifies the SOP Class associated with the Data Set.  SOP Class UIDs allowed for 
        /// media storage are specified in PS3.4 of the DICOM Standard - Media Storage Application Profiles.
        /// </summary>
        public string MediaStorageSopClassUid
        {
            get { return _metaInfo[DicomTags.MediaStorageSOPClassUID].GetString(0,""); }
            set { _metaInfo[DicomTags.MediaStorageSOPClassUID].Values = value; }
        }
        /// <summary>
        /// Uniquiely identifies the SOP Instance associated with the Data Set placed in the file and following the File Meta Information.
        /// </summary>
        public string MediaStorageSopInstanceUid
        {
            get { return _metaInfo[DicomTags.MediaStorageSOPInstanceUID].GetString(0,""); }
            set { _metaInfo[DicomTags.MediaStorageSOPInstanceUID].Values = value; }
        }
        /// <summary>
        /// Uniquely identifies the implementation which wrote this file and its content.  It provides an 
        /// unambiguous identification of the type of implementation which last wrote the file in the 
        /// event of interchagne problems.  It follows the same policies as defined by PS 3.7 of the DICOM Standard
        /// (association negotiation).
        /// </summary>
        public string ImplementationClassUid
        {
            get { return _metaInfo[DicomTags.ImplementationClassUID].GetString(0,""); }
            set { _metaInfo[DicomTags.ImplementationClassUID].Values = value; }
        }
        /// <summary>
        /// Identifies a version for an Implementation Class UID (002,0012) using up to 
        /// 16 characters of the repertoire.  It follows the same policies as defined in 
        /// PS 3.7 of the DICOM Standard (association negotiation).
        /// </summary>
        public string ImplementationVersionName
        {
            get { return _metaInfo[DicomTags.ImplementationVersionName].GetString(0,""); }
            set { _metaInfo[DicomTags.ImplementationVersionName].Values = value; }
        }
        /// <summary>
        /// Uniquely identifies the Transfer Syntax used to encode the following Data Set.  
        /// This Transfer Syntax does not apply to the File Meta Information.
        /// </summary>
        public string TransferSyntaxUid
        {
            get { return _metaInfo[DicomTags.TransferSyntaxUID].GetString(0,""); }
            set { _metaInfo[DicomTags.TransferSyntaxUID].Values = value; }
        }
        /// <summary>
        /// The DICOM Application Entity (AE) Title of the AE which wrote this file's 
        /// content (or last updated it).  If used, it allows the tracin of the source 
        /// of errors in the event of media interchange problems.  The policies associated
        /// with AE Titles are the same as those defined in PS 3.8 of the DICOM Standard. 
        /// </summary>
        public string SourceApplicationEntityTitle
        {
            get { return _metaInfo[DicomTags.SourceApplicationEntityTitle].GetString(0,""); }
            set { _metaInfo[DicomTags.SourceApplicationEntityTitle].Values = value; }
        }
        /// <summary>
        /// Identifies a version for an Implementation Class UID (002,0012) using up to 
        /// 16 characters of the repertoire.  It follows the same policies as defined in 
        /// PS 3.7 of the DICOM Standard (association negotiation).
        /// </summary>
        public string PrivateInformationCreatorUid
        {
            get { return _metaInfo[DicomTags.PrivateInformationCreatorUID].GetString(0,""); }
            set { _metaInfo[DicomTags.PrivateInformationCreatorUID].Values = value; }
        }
        #endregion

        #region Public Methods

        public void Load(DicomReadOptions options)
        {
            Load(null, options);
        }

        public void Load(uint stopTag, DicomReadOptions options)
        {
            DicomTag stopDicomTag = DicomTagDictionary.GetDicomTag(stopTag);
            if (stopDicomTag == null)
                stopDicomTag = new DicomTag(stopTag, "Bogus Tag", DicomVr.NONE, false, 1, 1, false);
            Load(stopDicomTag, options);
        }


        public void Load(DicomTag stopTag, DicomReadOptions options)
        {
            if (!File.Exists(Filename))
                throw new FileNotFoundException(Filename);

            if (stopTag == null)
                stopTag = new DicomTag(0xFFFFFFFF, "Bogus Tag", DicomVr.NONE, false, 1, 1, false);

            using (FileStream fs = File.OpenRead(Filename))
            {
                fs.Seek(128, SeekOrigin.Begin);
                DicomStreamReader dsr;
                if (!FileHasPart10Header(fs))
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    dsr = new DicomStreamReader(fs);
                    dsr.TransferSyntax = TransferSyntax.ImplicitVRLittleEndian;
                    dsr.Dataset = base._dataSet;
                    dsr.Read(stopTag, options);

                    TransferSyntax = TransferSyntax.ImplicitVRLittleEndian;
                    if (DataSet.Contains(DicomTags.SOPClassUID))
                        MediaStorageSopClassUid = DataSet[DicomTags.SOPClassUID].ToString();
                    if (DataSet.Contains(DicomTags.SOPInstanceUID))
                        MediaStorageSopInstanceUid = DataSet[DicomTags.SOPInstanceUID].ToString();

                    // TODO: put important tag values in the MetaHeader... like TransferSyntax, SopClassUid, etc.
                }
                else
                {
                    dsr = new DicomStreamReader(fs);
                    dsr.TransferSyntax = TransferSyntax.ExplicitVRLittleEndian;

                    dsr.Dataset = base._metaInfo;
                    dsr.Read(new DicomTag(0x0002FFFF, "Bogus Tag", DicomVr.UNvr, false, 1, 1, false), options);
                    dsr.Dataset = base._dataSet;
                    dsr.TransferSyntax = TransferSyntax;
                    dsr.Read(stopTag, options);
                }
            }
        }

        protected static bool FileHasPart10Header(FileStream fs)
        {
            return (!(fs.ReadByte() != (byte)'D' ||
                fs.ReadByte() != (byte)'I' ||
                fs.ReadByte() != (byte)'C' ||
                fs.ReadByte() != (byte)'M'));
        }

        public bool Save(DicomWriteOptions options)
        {
            using (FileStream fs = File.Create(Filename))
            {
                fs.Seek(128, SeekOrigin.Begin);
                fs.WriteByte((byte)'D');
                fs.WriteByte((byte)'I');
                fs.WriteByte((byte)'C');
                fs.WriteByte((byte)'M');

                DicomStreamWriter dsw = new DicomStreamWriter(fs);
                dsw.Write(TransferSyntax.ExplicitVRLittleEndian,
                    base._metaInfo, options | DicomWriteOptions.CalculateGroupLengths);
                
                dsw.Write(this.TransferSyntax,base._dataSet, options);
            }

            return true;
        }
        #endregion

        #region Dump
        public void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
        {
            sb.Append(prefix).AppendLine("File: " + Filename).AppendLine();
            sb.Append(prefix).Append("MetaInfo:").AppendLine();
            base._metaInfo.Dump(sb, prefix, options);
            sb.AppendLine().Append(prefix).Append("DataSet:").AppendLine();
            base._dataSet.Dump(sb, prefix, options);
            sb.AppendLine();
        }
        #endregion

    }
}

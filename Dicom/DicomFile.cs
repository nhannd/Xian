using System;
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
        /// <summary>
        /// Create a DicomFile instance from existing MetaInfo and DataSet.
        /// </summary>
        /// <param name="filename">The name for the file.</param>
        /// <param name="metaInfo">A <see cref="DicomAttributeCollection"/> for the MetaInfo (group 0x0002 attributes).</param>
        /// <param name="dataSet">A <see cref="DicomAttributeCollection"/> for the DataSet.</param>
        public DicomFile(String filename, DicomAttributeCollection metaInfo, DicomAttributeCollection dataSet)
        {
            _metaInfo = metaInfo;
            _dataSet = dataSet;
            _filename = filename;
        }

        /// <summary>
        /// Create a new empty DICOM Part 10 format file.
        /// </summary>
        /// <param name="filename"></param>
        public DicomFile(String filename)
        {
            _metaInfo = new DicomAttributeCollection(0x00020000, 0x0002FFFF);
            _dataSet = new DicomAttributeCollection(0x00040000, 0xFFFFFFFF);

            _filename = filename;
        }

        /// <summary>
        /// Create a new empty DICOM Part 10 format file.
        /// </summary>
        public DicomFile()
        {
            _metaInfo = new DicomAttributeCollection(0x00020000, 0x0002FFFF);
            _dataSet = new DicomAttributeCollection(0x00040000, 0xFFFFFFFF);

            _filename = String.Empty;
        }

        /// <summary>
        /// Creates a new DicomFile instance from an existing <see cref="DicomMessage"/> instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This routine assigns the existing <see cref="DicomMessage.DataSet"/> into the new 
        /// DicomFile instance.
        /// </para>
        /// <para>
        /// A new <see cref="DicomAttributeCollection"/> is created for the MetaInfo.  The 
        /// Media Storage SOP Instance UID, Media Storage SOP Class UID, Implementation Version Name,
        /// and Implementation Class UID tags are automatically set in the meta information.
        /// </para>
        /// </remarks>
        /// <param name="msg"></param>
        /// <param name="filename"></param>
        public DicomFile(DicomMessage msg, String filename)
        {
            _metaInfo = new DicomAttributeCollection(0x00020000,0x0002FFFF);
            _dataSet = msg.DataSet;

            MediaStorageSopInstanceUid = msg.AffectedSopInstanceUid;
            MediaStorageSopClassUid = msg.AffectedSopClassUid;
            ImplementationVersionName = DicomImplementation.Version;
            ImplementationClassUid = DicomImplementation.ClassUID.UID;

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
                String sopClassUid = MetaInfo[DicomTags.MediaStorageSopClassUid].GetString(0,String.Empty);

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
                String transferSyntaxUid = _metaInfo[DicomTags.TransferSyntaxUid];

                return TransferSyntax.GetTransferSyntax(transferSyntaxUid);
            }
            set
            {
                _metaInfo[DicomTags.TransferSyntaxUid].SetStringValue(value.UidString);
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
            get { return _metaInfo[DicomTags.MediaStorageSopClassUid].GetString(0,String.Empty); }
            set { _metaInfo[DicomTags.MediaStorageSopClassUid].Values = value; }
        }
        /// <summary>
        /// Uniquiely identifies the SOP Instance associated with the Data Set placed in the file and following the File Meta Information.
        /// </summary>
        public string MediaStorageSopInstanceUid
        {
            get { return _metaInfo[DicomTags.MediaStorageSopInstanceUid].GetString(0,String.Empty); }
            set { _metaInfo[DicomTags.MediaStorageSopInstanceUid].Values = value; }
        }
        /// <summary>
        /// Uniquely identifies the implementation which wrote this file and its content.  It provides an 
        /// unambiguous identification of the type of implementation which last wrote the file in the 
        /// event of interchagne problems.  It follows the same policies as defined by PS 3.7 of the DICOM Standard
        /// (association negotiation).
        /// </summary>
        public string ImplementationClassUid
        {
            get { return _metaInfo[DicomTags.ImplementationClassUid].GetString(0,String.Empty); }
            set { _metaInfo[DicomTags.ImplementationClassUid].Values = value; }
        }
        /// <summary>
        /// Identifies a version for an Implementation Class UID (002,0012) using up to 
        /// 16 characters of the repertoire.  It follows the same policies as defined in 
        /// PS 3.7 of the DICOM Standard (association negotiation).
        /// </summary>
        public string ImplementationVersionName
        {
            get { return _metaInfo[DicomTags.ImplementationVersionName].GetString(0,String.Empty); }
            set { _metaInfo[DicomTags.ImplementationVersionName].Values = value; }
        }
        /// <summary>
        /// Uniquely identifies the Transfer Syntax used to encode the following Data Set.  
        /// This Transfer Syntax does not apply to the File Meta Information.
        /// </summary>
        public string TransferSyntaxUid
        {
            get { return _metaInfo[DicomTags.TransferSyntaxUid].GetString(0,String.Empty); }
            set { _metaInfo[DicomTags.TransferSyntaxUid].Values = value; }
        }
        /// <summary>
        /// The DICOM Application Entity (AE) Title of the AE which wrote this file's 
        /// content (or last updated it).  If used, it allows the tracin of the source 
        /// of errors in the event of media interchange problems.  The policies associated
        /// with AE Titles are the same as those defined in PS 3.8 of the DICOM Standard. 
        /// </summary>
        public string SourceApplicationEntityTitle
        {
            get { return _metaInfo[DicomTags.SourceApplicationEntityTitle].GetString(0,String.Empty); }
            set { _metaInfo[DicomTags.SourceApplicationEntityTitle].Values = value; }
        }
        /// <summary>
        /// Identifies a version for an Implementation Class UID (002,0012) using up to 
        /// 16 characters of the repertoire.  It follows the same policies as defined in 
        /// PS 3.7 of the DICOM Standard (association negotiation).
        /// </summary>
        public string PrivateInformationCreatorUid
        {
            get { return _metaInfo[DicomTags.PrivateInformationCreatorUid].GetString(0,String.Empty); }
            set { _metaInfo[DicomTags.PrivateInformationCreatorUid].Values = value; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Load a DICOM file with the default <see cref="DicomReadOptions"/> set.
        /// </summary>
        /// <remarks>
        /// Note:  If the file does not contain DICM encoded in it, the routine will assume
        /// the file is not a Part 10 format file, and is instead encoded as just a DataSet
        /// with the transfer syntax set to Implicit VR Little Endian.
        /// </remarks>
        /// <param name="file">The path of the file to load.</param>
        public void Load(string file)
        {
            if (file == null) throw new ArgumentNullException("file");
            Filename = file;
            Load(DicomReadOptions.Default);
        }

        /// <summary>
        /// Load a DICOM file (as set by the <see cref="Filename"/> property) with the 
        /// default <see cref="DicomReadOptions"/> set.
        /// the file name to 
        /// </summary>
        /// <remarks>
        /// Note:  If the file does not contain DICM encoded in it, the routine will assume
        /// the file is not a Part 10 format file, and is instead encoded as just a DataSet
        /// with the transfer syntax set to Implicit VR Little Endian.
        /// </remarks>
        public void Load()
        {
            Load(DicomReadOptions.Default);
        }

        /// <summary>
        /// Load a DICOM file.
        /// </summary>
        /// <remarks>
        /// Note:  If the file does not contain DICM encoded in it, the routine will assume
        /// the file is not a Part 10 format file, and is instead encoded as just a DataSet
        /// with the transfer syntax set to Implicit VR Little Endian.
        /// </remarks>
        /// <param name="options">The options to use when reading the file.</param>
        /// <param name="file">The path of the file to load.</param>
        public void Load(DicomReadOptions options, string file)
        {
            if (file == null) throw new ArgumentNullException("file");
            Filename = file;
            Load(null, options);
        }

        /// <summary>
        /// Load a DICOM file (as set by the <see cref="Filename"/> property).
        /// </summary>
        /// <remarks>
        /// Note:  If the file does not contain DICM encoded in it, the routine will assume
        /// the file is not a Part 10 format file, and is instead encoded as just a DataSet
        /// with the transfer syntax set to Implicit VR Little Endian.
        /// </remarks>
        /// <param name="options">The options to use when reading the file.</param>
        public void Load(DicomReadOptions options)
        {
            Load(null, options);
        }

        /// <summary>
        /// Load a DICOM file.
        /// </summary>
        /// <remarks>
        /// Note:  If the file does not contain DICM encoded in it, the routine will assume
        /// the file is not a Part 10 format file, and is instead encoded as just a DataSet
        /// with the transfer syntax set to Implicit VR Little Endian.
        /// </remarks>
        /// <param name="stopTag">A tag to stop at when reading the file.  See the constants in <see cref="DicomTags"/>.</param>
        /// <param name="options">The options to use when reading the file.</param>
        /// <param name="file">The path of the file to load.</param>
        public void Load(uint stopTag, DicomReadOptions options, string file)
        {
            if (file == null) throw new ArgumentNullException("file");
            Filename = file;
            Load(stopTag, options);
        }

        /// <summary>
        /// Load a DICOM file (as set by the <see cref="Filename"/> property).
        /// </summary>
        /// <remarks>
        /// Note:  If the file does not contain DICM encoded in it, the routine will assume
        /// the file is not a Part 10 format file, and is instead encoded as just a DataSet
        /// with the transfer syntax set to Implicit VR Little Endian.
        /// </remarks>
        /// <param name="stopTag">A tag to stop at when reading the file.  See the constants in <see cref="DicomTags"/>.</param>
        /// <param name="options">The options to use when reading the file.</param>
        public void Load(uint stopTag, DicomReadOptions options)
        {
            DicomTag stopDicomTag = DicomTagDictionary.GetDicomTag(stopTag);
            if (stopDicomTag == null)
                stopDicomTag = new DicomTag(stopTag, "Bogus Tag", DicomVr.NONE, false, 1, 1, false);
            Load(stopDicomTag, options);
        }

        /// <summary>
        /// Load a DICOM file (as set by the <see cref="Filename"/> property).
        /// </summary>
        /// <remarks>
        /// Note:  If the file does not contain DICM encoded in it, the routine will assume
        /// the file is not a Part 10 format file, and is instead encoded as just a DataSet
        /// with the transfer syntax set to Implicit VR Little Endian.
        /// </remarks>
        /// <param name="stopTag"></param>
        /// <param name="options">The options to use when reading the file.</param>
        /// <param name="file">The path of the file to load.</param>
        public void Load(DicomTag stopTag, DicomReadOptions options, string file)
        {
            if (file == null) throw new ArgumentNullException("file");
            Filename = file;
            Load(stopTag, options);
        }

        /// <summary>
        /// Load a DICOM file (as set by the <see cref="Filename"/> property).
        /// </summary>
        /// <remarks>
        /// Note:  If the file does not contain DICM encoded in it, the routine will assume
        /// the file is not a Part 10 format file, and is instead encoded as just a DataSet
        /// with the transfer syntax set to Implicit VR Little Endian.
        /// </remarks>
        /// <param name="stopTag"></param>
        /// <param name="options">The options to use when reading the file.</param>
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
                    if (!Flags.IsSet(options,DicomReadOptions.ReadNonPart10Files))
                        throw new DicomException(String.Format("File is not part 10 format file: {0}",Filename));

                    fs.Seek(0, SeekOrigin.Begin);
                    dsr = new DicomStreamReader(fs);
                    dsr.TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;
                    dsr.Dataset = _dataSet;
                    DicomReadStatus stat = dsr.Read(stopTag, options);
                    if (stat != DicomReadStatus.Success)
                    {
                        DicomLogger.LogError("Unexpected error when reading file: {0}",Filename);
                        throw new DicomException("Unexpected read error with file: " + Filename);
                    }

                    TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;
                    if (DataSet.Contains(DicomTags.SopClassUid))
                        MediaStorageSopClassUid = DataSet[DicomTags.SopClassUid].ToString();
                    if (DataSet.Contains(DicomTags.SopInstanceUid))
                        MediaStorageSopInstanceUid = DataSet[DicomTags.SopInstanceUid].ToString();
                }
                else
                {
                    dsr = new DicomStreamReader(fs);
                    dsr.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

                    dsr.Dataset = _metaInfo;
                    DicomReadStatus stat = dsr.Read(new DicomTag(0x0002FFFF, "Bogus Tag", DicomVr.UNvr, false, 1, 1, false), options);
                    if (stat != DicomReadStatus.Success)
                    {
                        DicomLogger.LogError("Unexpected error when reading file Meta info for file: {0}", Filename);
                        throw new DicomException("Unexpected failure reading file Meta info for file: " + Filename);
                    }
                    dsr.Dataset = _dataSet;
                    dsr.TransferSyntax = TransferSyntax;
                    stat = dsr.Read(stopTag, options);
                    if (stat != DicomReadStatus.Success)
                    {
                        DicomLogger.LogError("Unexpected error when reading file: {0}", Filename);
                        throw new DicomException("Unexpected failure reading file: " + Filename);
                    }
                }
            }
        }

        /// <summary>
        /// Internal routine to see if the file is encoded as a DICOM Part 10 format file.
        /// </summary>
        /// <param name="fs">The <see cref="FileStream"/> being used to read the file.</param>
        /// <returns>true if the file has a DICOM Part 10 format file header.</returns>
        protected static bool FileHasPart10Header(FileStream fs)
        {
            return (!(fs.ReadByte() != (byte)'D' ||
                fs.ReadByte() != (byte)'I' ||
                fs.ReadByte() != (byte)'C' ||
                fs.ReadByte() != (byte)'M'));
        }

        /// <summary>
        /// Save the file as a DICOM Part 10 format file (as set by the <see cref="Filename"/> property) with 
        /// the default <see cref="DicomWriteOptions"/>.
        /// </summary>
        /// <returns>true on success, false on failure.</returns>
        public bool Save()
        {
            return Save(DicomWriteOptions.Default);
        }

        /// <summary>
        /// Save the file as a DICOM Part 10 format file (as set by the <see cref="Filename"/> property).
        /// </summary>
        /// <param name="options">The options to use when saving the file.</param>
        /// <returns></returns>
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
                dsw.Write(TransferSyntax.ExplicitVrLittleEndian,
                    _metaInfo, options | DicomWriteOptions.CalculateGroupLengths);
                
                dsw.Write(TransferSyntax,_dataSet, options);
            }

            return true;
        }
        /// <summary>
        /// Save the file as a DICOM Part 10 format file with the default <see cref="DicomWriteOptions"/>.
        /// </summary>
        /// <returns>true on success, false on failure.</returns>
        public bool Save(string file)
        {
            if (file == null) throw new ArgumentNullException("file");
            Filename = file;
            return Save(DicomWriteOptions.Default);
        }

        /// <summary>
        /// Save the file as a DICOM Part 10 format file.
        /// </summary>
        /// <param name="options">The options to use when saving the file.</param>
        /// <param name="file">The path of the file to save to.</param>
        /// <returns></returns>
        public bool Save(string file, DicomWriteOptions options)
        {
            if (file == null) throw new ArgumentNullException("file");

            Filename = file;

            using (FileStream fs = File.Create(Filename))
            {
                fs.Seek(128, SeekOrigin.Begin);
                fs.WriteByte((byte)'D');
                fs.WriteByte((byte)'I');
                fs.WriteByte((byte)'C');
                fs.WriteByte((byte)'M');

                DicomStreamWriter dsw = new DicomStreamWriter(fs);
                dsw.Write(TransferSyntax.ExplicitVrLittleEndian,
                    _metaInfo, options | DicomWriteOptions.CalculateGroupLengths);

                dsw.Write(TransferSyntax, _dataSet, options);
            }

            return true;
        }

        #endregion

        #region Dump
        /// <summary>
        /// Method to dump the contents of a file to a StringBuilder object.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="prefix"></param>
        /// <param name="options">The dump options.</param>
        public override void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
        {
            if (sb == null) throw new NullReferenceException("sb");
            sb.Append(prefix).AppendLine("File: " + Filename).AppendLine();
            sb.Append(prefix).Append("MetaInfo:").AppendLine();
            _metaInfo.Dump(sb, prefix, options);
            sb.AppendLine().Append(prefix).Append("DataSet:").AppendLine();
            _dataSet.Dump(sb, prefix, options);
            sb.AppendLine();
        }
        #endregion

    }
}

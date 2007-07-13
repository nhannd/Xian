using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.IO;

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
                String sopClassUid = base.DataSet[DicomTags.SOPClassUID].ToString();

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

        #region Public Methods

        public void Load(DicomReadOptions options)
        {
            Load(null, options);
        }

        public void Load(DicomTag stopTag, DicomReadOptions options)
        {
            using (FileStream fs = File.OpenRead(Filename))
            {
                fs.Seek(128, SeekOrigin.Begin);
                CheckFileHeader(fs);
                DicomStreamReader dsr = new DicomStreamReader(fs);
                dsr.TransferSyntax = TransferSyntax.ExplicitVRLittleEndian;

                dsr.Dataset = base._metaInfo;
                dsr.Read(new DicomTag(0x0002FFFF,"Bogus Tag",DicomVr.UNvr,false,1,1,false), options);
                dsr.Dataset = base._dataSet;
                dsr.TransferSyntax = TransferSyntax;
                if (stopTag == null)
                    stopTag = new DicomTag(0xFFFFFFFF, "Bogus Tag", DicomVr.UNvr, false, 1, 1, false);
                dsr.Read(stopTag, options);
            }
        }

        protected static void CheckFileHeader(FileStream fs)
        {
            if (fs.ReadByte() != (byte)'D' ||
                fs.ReadByte() != (byte)'I' ||
                fs.ReadByte() != (byte)'C' ||
                fs.ReadByte() != (byte)'M')
                throw new DicomException("Invalid DICOM file: " + fs.Name);
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

#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Web.Server.ImageServer
{

    internal class ImageServerSopDataSource : DicomMessageSopDataSource
    {
        private readonly string _path;
        private volatile bool _fullHeaderRetrieved;
        private bool _sopLoaded;

        public ImageServerSopDataSource(InstanceXml instanceXml, string path)
            : base(new DicomFile(path, new DicomAttributeCollection(), instanceXml.Collection))
        {
            //These don't get set properly for instance xml.
            DicomFile sourceFile = (DicomFile)SourceMessage;
            sourceFile.TransferSyntaxUid = instanceXml.TransferSyntax.UidString;
            sourceFile.MediaStorageSopInstanceUid = instanceXml.SopInstanceUid;
            sourceFile.MetaInfo[DicomTags.SopClassUid].SetString(0, instanceXml.SopClass.Uid);

            _path = path;
        }

        public bool SopLoaded
        {
            get { return _sopLoaded; }
        }
        private InstanceXmlDicomAttributeCollection AttributeCollection
        {
            get
            {
                 return (InstanceXmlDicomAttributeCollection)SourceMessage.DataSet;
            }
        }

        public override DicomAttribute this[DicomTag tag]
        {
            get
            {
                lock (SyncLock)
                {
                    if (NeedFullHeader(tag.TagValue))
                        GetFullHeader();

                    return base[tag];
                }
            }
        }

        public override DicomAttribute this[uint tag]
        {
            get
            {
                lock (SyncLock)
                {
                    if (NeedFullHeader(tag))
                        GetFullHeader();

                    return base[tag];
                }
            }
        }

        public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
        {
            lock (SyncLock)
            {
                if (NeedFullHeader(tag.TagValue))
                    GetFullHeader();

                return base.TryGetAttribute(tag, out attribute);
            }
        }

        public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
        {
            lock (SyncLock)
            {
                if (NeedFullHeader(tag))
                    GetFullHeader();

                return base.TryGetAttribute(tag, out attribute);
            }
        }

        public bool NeedFullHeader(uint tag)
        {
            if (_fullHeaderRetrieved)
                return false;

            if (AttributeCollection.IsTagExcluded(tag))
                return true;

            DicomAttribute attribute = base[tag];
            if (attribute is DicomAttributeSQ)
            {
                DicomSequenceItem[] items = attribute.Values as DicomSequenceItem[];
                if (items != null)
                {
                    foreach (DicomSequenceItem item in items)
                    {
                        if (!(item is InstanceXmlDicomSequenceItem)) continue;

                        if (((InstanceXmlDicomSequenceItem)item).HasExcludedTags(true))
                            return true;
                    }
                }
            }

            return false;
        }

        private void GetFullHeader()
        {
            if (_fullHeaderRetrieved) return;

            DicomFile imageHeader = new DicomFile();
            imageHeader.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default, _path);

            SourceMessage = imageHeader;
            _fullHeaderRetrieved = true;
        }

        /// <summary>
        /// Called by the base class to create a new <see cref="StandardSopDataSource.StandardSopFrameData"/>
        /// containing the data for a particular frame in the SOP instance.
        /// </summary>
        /// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
        /// <returns>A new <see cref="StandardSopDataSource.StandardSopFrameData"/> containing the data for a particular frame in the SOP instance.</returns>
        protected override StandardSopFrameData CreateFrameData(int frameNumber)
        {
            GetFullHeader();
            _sopLoaded = true;
            return new DicomMessageSopFrameData(frameNumber, this);
        }

        protected override void EnsureLoaded()
        {
        }
    }
}
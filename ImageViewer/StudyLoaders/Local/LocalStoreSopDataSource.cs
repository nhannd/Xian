#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.StudyLoaders.Local
{
    internal class LocalStoreSopDataSource : LocalSopDataSource
    {
        private readonly ISopInstance _sop;

        public LocalStoreSopDataSource(ISopInstance sop)
            : base(sop.FilePath)
        {
            _sop = sop;
        }

        public override string TransferSyntaxUid
        {
            get { return _sop.TransferSyntaxUid; }
        }

        public override string StudyInstanceUid
        {
            get { return _sop.GetParentSeries().GetParentStudy().StudyInstanceUid; }
        }
		
        public override string SeriesInstanceUid
        {
            get { return _sop.GetParentSeries().SeriesInstanceUid; }
        }

        public override string SopInstanceUid
        {
            get { return _sop.SopInstanceUid; }
        }
		
        public override string SopClassUid
        {
            get { return _sop.SopClassUid; }
        }

        public override DicomAttribute this[DicomTag tag]
        {
            get
            {
                //the _sop indexer is not thread-safe.
                lock (SyncLock)
                {
                    if (_sop.IsStoredTag(tag))
                        return _sop[tag];

                    return base[tag];
                }
            }
        }

        public override DicomAttribute this[uint tag]
        {
            get
            {
                //the _sop indexer is not thread-safe.
                lock (SyncLock)
                {
                    if (_sop.IsStoredTag(tag))
                        return _sop[tag];

                    return base[tag];
                }
            }
        }


        public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
        {
            lock (SyncLock)
            {
                if (_sop.IsStoredTag(tag))
                {
                    attribute = _sop[tag];
                    if (!attribute.IsEmpty)
                        return true;
                }

                return base.TryGetAttribute(tag, out attribute);
            }
        }

        public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
        {
            lock (SyncLock)
            {
                if (_sop.IsStoredTag(tag))
                {
                    attribute = _sop[tag];
                    if (!attribute.IsEmpty)
                        return true;
                }

                return base.TryGetAttribute(tag, out attribute);
            }
        }
    }
}
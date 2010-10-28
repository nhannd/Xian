#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile.CreateStudy
{
	/// <summary>
	/// Class for updating the Series and Sop Instance UIDs within a study.
	/// </summary>
	public class SeriesSopUpdateCommand : BaseImageLevelUpdateCommand
	{
		private readonly UidMapper _uidMapper;
        private readonly StudyStorageLocation _originalStudy;
        private readonly StudyStorageLocation _targetStudy;

	    #region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public SeriesSopUpdateCommand(StudyStorageLocation originalStudy, StudyStorageLocation targetStudy, UidMapper uidMapper)
			: base("SeriesSopUpdateCommand")
		{
		    _originalStudy = originalStudy;
		    _targetStudy = targetStudy;
			_uidMapper = uidMapper;
		}
		#endregion

		public override bool Apply(DicomFile file)
		{
            if (_uidMapper == null)
                return true; // Nothing to do

			string oldSeriesUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
			string oldSopUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

			string newSeriesUid;
            if (_uidMapper.ContainsSeries(oldSeriesUid))
                newSeriesUid = _uidMapper.FindNewSeriesUid(oldSeriesUid);
            else
            {
                newSeriesUid = DicomUid.GenerateUid().UID;
                _uidMapper.AddSeries(_originalStudy.StudyInstanceUid, _targetStudy.StudyInstanceUid, oldSeriesUid, newSeriesUid);
            }

			string newSopInstanceUid;
			if (_uidMapper.ContainsSop(oldSopUid))
				newSopInstanceUid = _uidMapper.FindNewSopUid(oldSopUid);
			else
			{
				newSopInstanceUid = DicomUid.GenerateUid().UID;
				_uidMapper.AddSop(oldSopUid, newSopInstanceUid);
			}

			file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(newSeriesUid);
			file.DataSet[DicomTags.SopInstanceUid].SetStringValue(newSopInstanceUid);
			file.MediaStorageSopInstanceUid = newSopInstanceUid;

            // add Source Image Sequence
            AddSourceImageSequence(file, oldSopUid);

		    return true;
		}

	    private void AddSourceImageSequence(DicomFile file, string oldSopUid)
	    {
	        DicomAttributeSQ sourceImageSq;
	        if (!file.DataSet.Contains(DicomTags.SourceImageSequence))
	        {
	            sourceImageSq = new DicomAttributeSQ(DicomTags.SourceImageSequence);
	            file.DataSet[DicomTags.SourceImageSequence] = sourceImageSq;
	        }
	        else
	            sourceImageSq = file.DataSet[DicomTags.SourceImageSequence] as DicomAttributeSQ;

	        DicomSequenceItem item = new DicomSequenceItem();
	        item[DicomTags.ReferencedSopClassUid].SetStringValue(file.SopClass.Uid);
	        item[DicomTags.ReferencedSopInstanceUid].SetStringValue(oldSopUid);
	        sourceImageSq.AddSequenceItem(item);
	    }
	}
}

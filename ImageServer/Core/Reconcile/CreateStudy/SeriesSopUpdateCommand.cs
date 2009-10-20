#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

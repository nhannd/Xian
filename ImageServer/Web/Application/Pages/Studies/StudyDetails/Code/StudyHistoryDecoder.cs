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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Provides methods to decode the information in a <see cref="StudyHistory"/> record.
    /// </summary>
    static class StudyHistoryRecordDecoder
    {
        public static ReconcileHistoryRecord ReadReconcileRecord(StudyHistory historyRecord)
        {
            Platform.CheckTrue(historyRecord.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled,
                               "History record has invalid history record type");

            ReconcileHistoryRecord record = new ReconcileHistoryRecord();
            record.InsertTime = historyRecord.InsertTime;
            record.StudyStorageLocation = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(historyRecord.StudyStorageKey))[0];
            StudyReconcileDescriptorParser parser = new StudyReconcileDescriptorParser();
            record.UpdateDescription = parser.Parse(historyRecord.ChangeDescription);
            return record;
        }

        public static WebEditStudyHistoryRecord ReadEditRecord(StudyHistory historyRecord)
        {
            Platform.CheckTrue(historyRecord.StudyHistoryTypeEnum == StudyHistoryTypeEnum.WebEdited,
                               "History record has invalid history record type");

            WebEditStudyHistoryRecord record = new WebEditStudyHistoryRecord();
            record.InsertTime = historyRecord.InsertTime;
            record.StudyStorageLocation = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(historyRecord.StudyStorageKey))[0];
            record.UpdateDescription = XmlUtils.Deserialize<WebEditStudyHistoryChangeDescription>(historyRecord.ChangeDescription);
            return record;
        }
    }
    
    internal class StudyHistoryRecordBase
    {
        public DateTime InsertTime;
        public StudyStorageLocation StudyStorageLocation;
    }
}

#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.Tier1RetentionAction
{
    public class Tier1RetentionActionItem : IActionItem<ServerActionContext>
    {
        private string _failureReason = "Success";
        private readonly double _time;
        private readonly string _timeUnits;
        private static readonly FilesystemQueueTypeEnum _queueType = FilesystemQueueTypeEnum.GetEnum("TierMigrate");

        public Tier1RetentionActionItem(double time, string timeUnits)
        {
            _time = time;
            _timeUnits = timeUnits.ToLower();
        }
        public bool Execute(ServerActionContext context)
        {
            DateTime scheduledTime = Platform.Time;

            if (_timeUnits.Equals("hours"))
                scheduledTime = scheduledTime.AddHours(_time);
            else if (_timeUnits.Equals("days"))
                scheduledTime = scheduledTime.AddDays(_time);
            else if (_timeUnits.Equals("weeks"))
                scheduledTime = scheduledTime.AddDays(_time * 7f);
            else if (_timeUnits.Equals("months"))
                scheduledTime = scheduledTime.AddMonths((int)_time);
            else if (_timeUnits.Equals("patientage"))
            {
                DateTime birthDate = context.Message.DataSet[DicomTags.PatientsBirthDate].GetDateTime(0, Platform.Time);

                scheduledTime = birthDate.AddYears((int)_time);
            }
            else
            {
                _failureReason = String.Format("Unexpected time units for tier1-retention action item: {0}", _timeUnits);
                return false;
            }

            context.CommandProcessor.AddCommand(new InsertFilesystemQueueCommand(_queueType, context.FilesystemKey, context.StudyLocationKey, scheduledTime));

            return true;
        }

        public string FailureReason
        {
            get { return _failureReason; }
        }
    }
}

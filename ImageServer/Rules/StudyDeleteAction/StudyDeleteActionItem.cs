#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.StudyDeleteAction
{


    public class StudyDeleteActionItem : ServerActionItemBase
    {

        private readonly int _offsetTime;
        private readonly TimeUnit _units;
        private readonly ReferenceValue _referenceValue;

        private static readonly FilesystemQueueTypeEnum _queueType = FilesystemQueueTypeEnum.GetEnum("DeleteStudy");

        public StudyDeleteActionItem(int time, TimeUnit unit, string refValue)
            : base("Study Delete action")
        {
            _offsetTime = time;
            _units = unit;
            _referenceValue = new ReferenceValue(refValue);
        }

        protected override bool OnExecute(ServerActionContext context)
        {
            _referenceValue.Context = context;

            DateTime? scheduledTime = ResolveTime(context, _offsetTime, _units, _referenceValue, Platform.Time);
            if (scheduledTime != null)
            {
                int minRetention = RuleSettings.Default.MIN_RETENTION_MINUTES;
                if (scheduledTime < Platform.Time.AddMinutes(minRetention))
                {
                    DateTime preferredScheduledTime = Platform.Time.AddMinutes(minRetention);
                    Platform.Log(LogLevel.Warn, "Study Delete: calculated delete time is {1}. Min retention time = {0} minutes ==> preferred delete time is {2}", minRetention, scheduledTime, preferredScheduledTime);
                    scheduledTime = preferredScheduledTime;
                }
                    

                Platform.Log(LogLevel.Debug, "Study Delete: This study will be deleted on {0}", scheduledTime);
                context.CommandProcessor.AddCommand(new InsertFilesystemQueueCommand(_queueType, context.FilesystemKey, context.StudyLocationKey, scheduledTime.Value));

            }

            return scheduledTime != null; 
            
        }

        #region IActionItem<ServerActionContext> Members


        #endregion
    }
}

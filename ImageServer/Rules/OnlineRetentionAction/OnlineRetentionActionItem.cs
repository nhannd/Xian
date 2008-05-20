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
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.OnlineRetentionAction
{
    public class OnlineRetentionActionItem : ServerActionItemBase
    {
        private static readonly FilesystemQueueTypeEnum _queueType = FilesystemQueueTypeEnum.GetEnum("PurgeStudy");
        private readonly Expression _exprScheduledTime;

        private readonly int _offsetTime;
        private readonly TimeUnit _units;

        public OnlineRetentionActionItem(int time, TimeUnit unit)
            : this(time, unit, null)
        {
        }

        public OnlineRetentionActionItem(int time, TimeUnit unit, Expression exprScheduledTime)
            : base("Study Delete action")
        {
            _offsetTime = time;
            _units = unit;
            _exprScheduledTime = exprScheduledTime;
        }

        protected override bool OnExecute(ServerActionContext context)
        {
            DateTime scheduledTime = Platform.Time;

            if (_exprScheduledTime != null)
            {
                scheduledTime = Evaluate(_exprScheduledTime, context, Platform.Time);
            }

            scheduledTime = CalculateOffsetTime(scheduledTime, _offsetTime, _units);
            DateTime preferredScheduledTime = Platform.Time.AddMinutes(RuleSettings.Default.MIN_RETENTION_MINUTES);

            if (scheduledTime < preferredScheduledTime)
            {
                Platform.Log(LogLevel.Warn,
                             "Online Retention: calculated scheduled delete time is {0}. ==> preferred time is {1}",
                             scheduledTime, preferredScheduledTime);
                scheduledTime = preferredScheduledTime;
            }

            Platform.Log(LogLevel.Debug, "Online Retention: This study will be purged on {0}", scheduledTime);
            context.CommandProcessor.AddCommand(
                new InsertFilesystemQueueCommand(_queueType, context.FilesystemKey, context.StudyLocationKey,
                                                 scheduledTime));


            return true;
        }
    }
}
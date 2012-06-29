#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.OnlineRetentionAction
{
    public class OnlineRetentionActionItem : ActionItemBase<ServerActionContext>
    {
        private static readonly FilesystemQueueTypeEnum _queueType = FilesystemQueueTypeEnum.PurgeStudy;
        private readonly Expression _exprScheduledTime;

        private readonly int _offsetTime;
        private readonly TimeUnit _units;

        public OnlineRetentionActionItem(int time, TimeUnit unit)
            : this(time, unit, null)
        {
        }

        public OnlineRetentionActionItem(int time, TimeUnit unit, Expression exprScheduledTime)
            : base("Online Retention action")
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

            context.CommandProcessor.AddCommand(
                new InsertFilesystemQueueCommand(_queueType, context.FilesystemKey, context.StudyLocationKey,
                                                 scheduledTime, null));


            return true;
        }
    }
}
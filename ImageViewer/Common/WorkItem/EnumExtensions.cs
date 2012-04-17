#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;


namespace ClearCanvas.ImageViewer.Common.WorkItem
{
	public static class EnumExtensions
	{
		public static string GetDescription(this WorkItemTypeEnum value)
		{
			switch (value)
			{
				case WorkItemTypeEnum.DicomRetrieve:
					return SR.WorkItemTypeEnumDicomRetrieve;
				case WorkItemTypeEnum.DicomSend:
					return SR.WorkItemTypeEnumDicomSend;
				case WorkItemTypeEnum.Import:
					return SR.WorkItemTypeEnumImport;
				case WorkItemTypeEnum.ReapplyRules:
					return SR.WorkItemTypeEnumReapplyRules;
				case WorkItemTypeEnum.ReIndex:
					return SR.WorkItemTypeEnumReIndex;
				case WorkItemTypeEnum.SeriesDelete:
					return SR.WorkItemTypeEnumSeriesDelete;
				case WorkItemTypeEnum.StudyDelete:
					return SR.WorkItemTypeEnumStudyDelete;
				case WorkItemTypeEnum.StudyProcess:
					return SR.WorkItemTypeEnumStudyProcess;
			}
			throw new NotImplementedException();
		}

		public static string GetDescription(this WorkItemStatusEnum value)
		{
			switch (value)
			{
				case WorkItemStatusEnum.Pending:
					return SR.WorkItemStatusEnumPending;
				case WorkItemStatusEnum.InProgress:
					return SR.WorkItemStatusEnumInProgress;
				case WorkItemStatusEnum.Complete:
					return SR.WorkItemStatusEnumComplete;
				case WorkItemStatusEnum.Idle:
					return SR.WorkItemStatusEnumIdle;
				case WorkItemStatusEnum.Deleted:
					return SR.WorkItemStatusEnumDeleted;
				case WorkItemStatusEnum.Canceled:
					return SR.WorkItemStatusEnumCanceled;
				case WorkItemStatusEnum.Failed:
					return SR.WorkItemStatusEnumFailed;
			}
			throw new NotImplementedException();
		}

		public static string GetDescription(this WorkItemPriorityEnum value)
		{
			switch (value)
			{
				case WorkItemPriorityEnum.Normal:
					return SR.WorkItemPriorityEnumNormal;
				case WorkItemPriorityEnum.Stat:
					return SR.WorkItemPriorityEnumStat;
			}
			throw new NotImplementedException();
		}

        public static string GetDescription(this ActivityTypeEnum value)
        {
            switch (value)
            {
                case ActivityTypeEnum.AutoRoute:
                    return SR.ActivityTypeEnumAutoRoute;
                case ActivityTypeEnum.DicomReceive:
                    return SR.ActivityTypeEnumDicomReceive;
                case ActivityTypeEnum.DicomRetrieve:
                    return SR.ActivityTypeEnumDicomRetrieve;
                case ActivityTypeEnum.DicomSendSop:
                    return SR.ActivityTypeEnumDicomSendSop;
                case ActivityTypeEnum.DicomSendSeries:
                    return SR.ActivityTypeEnumDicomSendSeries;
                case ActivityTypeEnum.DicomSendStudy:
                    return SR.ActivityTypeEnumDicomSendStudy;
                case ActivityTypeEnum.PublishFiles:
                    return SR.ActivityTypeEnumPublishFiles;
                case ActivityTypeEnum.ImportFiles:
                    return SR.ActivityTypeEnumImportFiles;
                case ActivityTypeEnum.ImportStudy:
                    return SR.ActivityTypeEnumImportStudy;
                case ActivityTypeEnum.ReIndex:
                    return SR.ActivityTypeEnumReIndex;
                case ActivityTypeEnum.ReapplyRules:
                    return SR.ActivityTypeEnumReapplyRules;
            }
            throw new NotImplementedException();
        }
	}
}

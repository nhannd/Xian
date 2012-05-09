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
                case WorkItemStatusEnum.DeleteInProgress:
                    return SR.WorkItemStatusEnumDeleteInProgress;
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
                case WorkItemPriorityEnum.High:
                    return SR.WorkItemPriorityEnumHigh;
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
                case ActivityTypeEnum.DeleteStudy:
                    return SR.ActivityTypeEnumDeleteStudy;
                case ActivityTypeEnum.DeleteSeries:
                    return SR.ActivityTypeEnumDeleteeries;
            }
            throw new NotImplementedException();
        }

		public static bool CancellationCanResultInPartialStudy(this ActivityTypeEnum value)
		{
			switch (value)
			{
				case ActivityTypeEnum.DicomReceive:
				case ActivityTypeEnum.ImportStudy:
				case ActivityTypeEnum.ImportFiles:
				case ActivityTypeEnum.DicomRetrieve:
				case ActivityTypeEnum.DicomSendStudy:
				case ActivityTypeEnum.DicomSendSeries:
				case ActivityTypeEnum.DicomSendSop:
				case ActivityTypeEnum.AutoRoute:
				case ActivityTypeEnum.PublishFiles:
					return true;

				default:
					return false;
			}
		}
	}
}

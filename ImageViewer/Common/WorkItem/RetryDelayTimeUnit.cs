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
	public enum RetryDelayTimeUnit
	{
		Seconds,
		Minutes,
	}

	public static class RetryDelayTimeUnitExtensions
	{
		public static string GetDescription(this RetryDelayTimeUnit value)
		{
			switch (value)
			{
				case RetryDelayTimeUnit.Seconds:
					return SR.RetryDelayTimeUnitSeconds;
				case RetryDelayTimeUnit.Minutes:
					return SR.RetryDelayTimeUnitMinutes;
				default:
					throw new ArgumentOutOfRangeException("value");
			}
		}
	}
}

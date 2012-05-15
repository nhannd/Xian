#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Common
{
	public enum TimeUnit
	{
		Weeks,
		Days,
		Hours,
		Minutes,
	}

	public static class TimeUnitEnumExtensions
	{
		/// <summary>
		/// Gets a user-friendly description of this value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetDescription(this TimeUnit value)
		{
			switch (value)
			{
				case TimeUnit.Weeks:
					return SR.TimeUnitWeeks;
				case TimeUnit.Days:
					return SR.TimeUnitDays;
				case TimeUnit.Hours:
					return SR.TimeUnitHours;
				case TimeUnit.Minutes:
					return SR.TimeUnitMinutes;
			}
			throw new ArgumentOutOfRangeException();
		}
	}


}

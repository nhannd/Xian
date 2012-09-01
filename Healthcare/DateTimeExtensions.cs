#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;


namespace ClearCanvas.Healthcare
{
	public enum DateTimePrecision
	{
		Day,
		Hour,
		Minute,
		Second
	}

	public static class DateTimeExtensions
	{
		/// <summary>
		/// Returns a DateTime truncated to the specified precision.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="precision"> </param>
		/// <returns></returns>
		public static DateTime Truncate(this DateTime t, DateTimePrecision precision)
		{
			switch (precision)
			{
				case DateTimePrecision.Day:
					return t.Date;
				case DateTimePrecision.Hour:
					return new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0);
				case DateTimePrecision.Minute:
					return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0);
				case DateTimePrecision.Second:
					return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
			}
			throw new ArgumentOutOfRangeException("precision");
		}
	}
}

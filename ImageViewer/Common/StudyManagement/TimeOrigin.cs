#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	public enum TimeOrigin
	{
		ReceivedDate,
		StudyDate,
	}

	public static class TimeOriginEnumExtensions
	{
		/// <summary>
		/// Gets a user-friendly description of this value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetDescription(this TimeOrigin value)
		{
			switch (value)
			{
				case TimeOrigin.ReceivedDate:
					return SR.TimeOriginReceivedDate;
				case TimeOrigin.StudyDate:
					return SR.TimeOriginStudyDate;
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}

#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines the possible levels for alerts.
	/// </summary>
	public enum AlertLevel
	{
		/// <summary>
		/// An informational alert notifies the user of an event that is not a problem.
		/// </summary>
		Info,

		/// <summary>
		/// A warning alert notifies the user of a potentially problematic event.
		/// </summary>
		Warning,

		/// <summary>
		/// An error alert notifies the user of a failure which will likely require some corrective action.
		/// </summary>
		Error
	}

	internal static class AlertLevelExtensions
	{
		/// <summary>
		/// Gets the icon corresponding to the specified alert level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public static IconSet GetIcon(this AlertLevel level)
		{
			switch (level)
			{
				case AlertLevel.Info:
					return new IconSet("InfoMini.png", "InfoSmall.png", "InfoMedium.png");
				case AlertLevel.Warning:
					return new IconSet("WarningMini.png", "WarningSmall.png", "WarningMedium.png");
				case AlertLevel.Error:
					return new IconSet("ErrorMini.png", "ErrorSmall.png", "ErrorMedium.png");
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}

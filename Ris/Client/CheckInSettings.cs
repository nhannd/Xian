#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides application settings for check-in.
	/// </summary>
	/// <remarks>
	/// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
	/// settings need to be manually added to this class.
	/// </remarks>
	[SettingsGroupDescription("Configures behaviour of check-in procedures.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	public sealed class CheckInSettings : global::System.Configuration.ApplicationSettingsBase
	{
		private static CheckInSettings defaultInstance = ((CheckInSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new CheckInSettings())));
		
		public CheckInSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public static CheckInSettings Default {
			get {
				return defaultInstance;
			}
		}
		
		/// <summary>
		/// Specifies how early a procedure can be checked in without triggering a warning. (in minutes).
		/// </summary>
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.SettingsDescriptionAttribute("Specifies how early a procedure can be checked in without triggering a warning. (in minutes)")]
		[global::System.Configuration.DefaultSettingValueAttribute("120")]
		public int EarlyCheckInWarningThreshold {
			get {
				return ((int)(this["EarlyCheckInWarningThreshold"]));
			}
		}

		/// <summary>
		/// Specifies how late a procedure can be checked in without triggering a warning. (in minutes).
		/// </summary>
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.SettingsDescriptionAttribute("Specifies how late a procedure can be checked in without triggering a warning. (in minutes)")]
		[global::System.Configuration.DefaultSettingValueAttribute("180")]
		public int LateCheckInWarningThreshold {
			get {
				return ((int)(this["LateCheckInWarningThreshold"]));
			}
		}

		public enum ValidateResult { Success, TooEarly, TooLate, NotScheduled }

		public static ValidateResult Validate(DateTime? scheduledTime, DateTime checkInTime, out string message)
		{
			message = "";
			if (scheduledTime == null)
			{
				message = SR.MessageCheckInUnscheduledProcedure;
				return ValidateResult.NotScheduled;
			}

			var earlyBound = scheduledTime.Value.AddMinutes(-Default.EarlyCheckInWarningThreshold);
			var lateBound = scheduledTime.Value.AddMinutes(Default.LateCheckInWarningThreshold);

			if (checkInTime < earlyBound)
			{
				var threshold = TimeSpan.FromMinutes(Default.EarlyCheckInWarningThreshold);
				message = string.Format(SR.MessageAlertCheckingInTooEarly, TimeSpanFormat.FormatDescriptive(threshold));
				return ValidateResult.TooEarly;
			}

			if (checkInTime > lateBound)
			{
				var threshold = TimeSpan.FromMinutes(Default.LateCheckInWarningThreshold);
				message = string.Format(SR.MessageAlertCheckingInTooLate, TimeSpanFormat.FormatDescriptive(threshold));
				return ValidateResult.TooLate;
			}

			return ValidateResult.Success;
		}
	}
}

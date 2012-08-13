#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.Ris.Shreds.HL7
{
	[SettingsGroupDescription("Settings that configure the behaviour of the Logical HL7 Event Shred.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class LogicalHL7EventShredSettings
	{
		///<summary>
		/// Public constructor.  Server-side settings classes should be instantiated via constructor rather
		/// than using the <see cref="Default"/> property to avoid creating a static instance.
		///</summary>
		public LogicalHL7EventShredSettings()
		{
			// Note: server-side settings classes do not register in the <see cref="ApplicationSettingsRegistry"/>
		}
	}
}

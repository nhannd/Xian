#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.Healthcare.Owls.Views
{
	/// <summary>
	/// Defines settings that configure behaviour of OWLS.
	/// </summary>
	[SettingsGroupDescription("Configures behaviour of OWLS.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class OwlsViewSettings
	{
		///<summary>
		/// Public constructor.  Server-side settings classes should be instantiated via constructor rather
		/// than using the <see cref="OwlsViewSettings.Default"/> property to avoid creating a static instance.
		///</summary>
		public OwlsViewSettings()
		{
		}
	}
}

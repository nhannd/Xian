#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Server.ShredHost
{
	/// <summary>
	/// Defines the method to be called to manually migrate the legacy shred configuration section class into the new settings class.
	/// </summary>
	public interface IMigrateLegacyShredConfigSection
	{
		void Migrate();
	}

	/// <summary>
	/// Declares the section path in legacy shred configuration files that is handled by this settings type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class LegacyShredConfigSectionAttribute : Attribute
	{
		private readonly string _sourceSectionPath;

		public LegacyShredConfigSectionAttribute(string sourceSectionPath)
		{
			_sourceSectionPath = sourceSectionPath;
		}

		public static bool IsMatchingLegacyShredConfigSectionType(Type configurationSectionType, string sectionPath)
		{
			foreach (LegacyShredConfigSectionAttribute attribute in GetCustomAttributes(configurationSectionType, typeof (LegacyShredConfigSectionAttribute), false))
			{
				if (attribute._sourceSectionPath == sectionPath)
					return true;
			}
			return false;
		}
	}
}
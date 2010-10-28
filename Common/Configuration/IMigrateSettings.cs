#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Configuration
{
	public enum MigrationScope
	{
		User,
		Shared
	}
	
	public class UserSettingsMigrationDisabledAttribute : Attribute
    {}

    public class SharedSettingsMigrationDisabledAttribute : Attribute
    {}

    public class SettingsPropertyMigrationValues
    {
		public SettingsPropertyMigrationValues(string propertyName, MigrationScope migrationScope, object currentValue, object previousValue)
        {
			PropertyName = propertyName;
			MigrationScope = migrationScope;
            CurrentValue = currentValue;
            PreviousValue = previousValue;
        }

		public MigrationScope MigrationScope { get; private set; }
		public string PropertyName { get; private set; }
        public object PreviousValue { get; private set; }
        public object CurrentValue { get; set; }
    }

	public interface IMigrateSettings
    {
        void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues);
    }
}
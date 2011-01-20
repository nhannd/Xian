#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Server.ShredHost
{
	public class ShredSettingsMigrator
	{
		private class ConfigurationSectionEntry
		{
			public ConfigurationSectionEntry(ConfigurationSectionGroupPath parentPath, ConfigurationSection section)
			{
				ParentPath = parentPath;
				Section = section;
			}

			public readonly ConfigurationSectionGroupPath ParentPath;
			public readonly ConfigurationSection Section;
		}

		private static readonly List<Type> _shredSettingsTypes = ListShredSettingsTypes();

		private static List<Type> ListShredSettingsTypes()
		{
			var types = new List<Type>();

			List<Assembly> assemblies = CollectionUtils.Map(Platform.PluginManager.Plugins, (PluginInfo p) => p.Assembly);
			assemblies.Add(typeof(ShredSettingsMigrator).Assembly);
			foreach (var assembly in assemblies)
			{
				foreach (Type t in assembly.GetTypes())
				{
					if (t.IsSubclassOf(typeof(ConfigurationSection)) && !t.IsAbstract)
						types.Add(t);
				}
			}

			return types;
		}

		private static bool IsShredSettingsClass(ConfigurationSection section)
		{
			return _shredSettingsTypes.Contains(section.GetType());
		}

		private static IEnumerable<ConfigurationSectionEntry> GetConfigurationSections(Configuration configuration)
		{
			ConfigurationSectionGroupPath rootPath = ConfigurationSectionGroupPath.Root;
			foreach (var childSection in GetConfigurationSections(configuration.RootSectionGroup, rootPath, true))
				yield return childSection;
		}

		private static IEnumerable<ConfigurationSectionEntry> GetConfigurationSections(ConfigurationSectionGroup group, ConfigurationSectionGroupPath groupPath, bool recursive)
		{
			if (recursive)
			{
				foreach (ConfigurationSectionGroup childGroup in group.SectionGroups)
				{
					foreach (var sectionEntry in GetConfigurationSections(childGroup, groupPath.GetChildGroupPath(childGroup.Name), true))
						yield return sectionEntry;
				}
			}

			foreach (ConfigurationSection section in group.Sections)
				yield return new ConfigurationSectionEntry(groupPath, section);
		}

		private static void MigrateSection(ConfigurationSection sourceSection, ConfigurationSectionGroupPath groupPath, Configuration destinationConfiguration)
		{
			if (sourceSection.GetType().IsDefined(typeof(SharedSettingsMigrationDisabledAttribute), false))
				return; //disabled

			var destinationGroup = groupPath.GetSectionGroup(destinationConfiguration, true);

			var destinationSection = destinationGroup.Sections[sourceSection.SectionInformation.Name];
			if (destinationSection == null)
			{
				destinationSection = (ConfigurationSection)Activator.CreateInstance(sourceSection.GetType(), true);
				destinationGroup.Sections.Add(sourceSection.SectionInformation.Name, destinationSection);
			}

			var customMigrator = sourceSection as IMigrateSettings;
			foreach (PropertyInformation sourceProperty in sourceSection.ElementInformation.Properties)
			{
				var destinationProperty = destinationSection.ElementInformation.Properties[sourceProperty.Name];
				if (destinationProperty == null)
					continue;

				if (customMigrator != null)
				{
					var migrationValues = new SettingsPropertyMigrationValues(
						sourceProperty.Name, MigrationScope.Shared, destinationProperty.Value, sourceProperty.Value);

					customMigrator.MigrateSettingsProperty(migrationValues);
					if (!Equals(migrationValues.CurrentValue, destinationProperty.Value))
					{
						destinationSection.SectionInformation.ForceSave = true; 
						destinationProperty.Value = migrationValues.CurrentValue;
					}
				}
				else
				{
					destinationSection.SectionInformation.ForceSave = true;
					destinationProperty.Value = sourceProperty.Value;
				}
			}
		}

		public static void MigrateAll(string previousExeConfigFilename)
		{
			Configuration previousConfiguration = SystemConfigurationHelper.GetExeConfiguration(previousExeConfigFilename);
			Configuration currentConfiguration = SystemConfigurationHelper.GetExeConfiguration();

			foreach (var sectionEntry in GetConfigurationSections(previousConfiguration))
			{
				if (IsShredSettingsClass(sectionEntry.Section))
					MigrateSection(sectionEntry.Section, sectionEntry.ParentPath, currentConfiguration);
			}

			currentConfiguration.Save(ConfigurationSaveMode.Full);
		}
	}
}

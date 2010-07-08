using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
	internal class SettingsPropertyExtensions
	{
		public static bool IsUserScoped(SettingsProperty property)
		{
			return CollectionUtils.Contains(property.Attributes.Values, attribute => attribute is UserScopedSettingAttribute);
		}

		public static bool IsAppScoped(SettingsProperty property)
		{
			return CollectionUtils.Contains(property.Attributes.Values, attribute => attribute is ApplicationScopedSettingAttribute);
		}

		public static string GetDescription(SettingsProperty property)
		{
			SettingsDescriptionAttribute a = CollectionUtils.SelectFirst(property.Attributes,
						attribute => attribute is SettingsDescriptionAttribute) as SettingsDescriptionAttribute;

			return a == null ? "" : a.Description;
		}
	}

	internal class SettingsPropertyHelper 
	{
		public static SettingScope GetScope(SettingsProperty property)
		{
			return SettingsPropertyExtensions.IsUserScoped(property) ? SettingScope.User : SettingScope.Application;
		}

		public static SettingsPropertyDescriptor GetDescriptor(SettingsProperty property)
		{
			return new SettingsPropertyDescriptor(property.Name, property.PropertyType.FullName,
				SettingsPropertyExtensions.GetDescription(property), GetScope(property), property.DefaultValue as string);
		}
	}
}

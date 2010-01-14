using System.Configuration;
using System.Xml;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	[ExtensionOf(typeof(EntityValidationRuleSetSourceExtensionPoint))]
	public class Foo : XmlValidationRuleSetSource
	{
		protected override XmlDocument RuleSetDocument
		{
			get
			{
				var settings = new EntityValidationRulesSettings();
				var doc = new XmlDocument();
				doc.LoadXml(settings.CustomRulesXml);
				return doc;
			}
		}
	}

	[SettingsGroupDescription("Defines custom entity validation rules for the Healthcare model.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class EntityValidationRulesSettings
	{
		///<summary>
		/// Public constructor.
		///</summary>
		public EntityValidationRulesSettings()
		{
		}
	}
}

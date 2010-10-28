#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using System.Xml;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	[ExtensionOf(typeof(EntityValidationRuleSetSourceExtensionPoint))]
	public class HealthcareXmlValidationRuleSetSource : XmlValidationRuleSetSource
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

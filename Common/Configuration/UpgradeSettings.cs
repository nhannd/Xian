#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using System.Xml;
using System.Threading;

namespace ClearCanvas.Common.Configuration
{
	[SettingsGroupDescription("Settings related to application upgrades.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	[UserSettingsMigrationDisabled]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class UpgradeSettings
	{
		private UpgradeSettings()
		{
		}

		public bool UnitTesting = false;

		internal static void CheckUserUpgradeEnabled()
		{
			if (!IsUserUpgradeEnabled())
				throw new InvalidOperationException("User upgrade is currently disabled.");
		}

		internal static bool IsUserUpgradeEnabled()
		{
			return Default.UnitTesting || Default.UserUpgradeEnabled;
		}

		public override void Upgrade()
		{
			//Disable user settings upgrade.
		}

		private XmlElement CompletedUserUpgradeSteps
		{
			get
			{
				const string documentElementName = "completed-user-upgrade-steps";

				if (CompletedUserUpgradeStepsXml == null || CompletedUserUpgradeStepsXml.DocumentElement == null || CompletedUserUpgradeStepsXml.DocumentElement.Name != documentElementName)
				{
					CompletedUserUpgradeStepsXml = new XmlDocument();
					XmlElement documentElement = CompletedUserUpgradeStepsXml.CreateElement(documentElementName);
					CompletedUserUpgradeStepsXml.AppendChild(documentElement);
					return documentElement;
				}

				return CompletedUserUpgradeStepsXml.DocumentElement;
			}
		}

		private XmlElement GetUserUpgradeStepElement(string identifier, bool create)
		{
			const string userUpgradeStepElementName = "user-upgrade-step";
			const string identifierAttributeName = "identifier";

			string xPath = String.Format("{0}[@{1}='{2}']", userUpgradeStepElementName, identifierAttributeName, identifier);
			XmlElement element = CompletedUserUpgradeSteps.SelectSingleNode(xPath) as XmlElement;

			if (element == null && create)
			{
				element = CompletedUserUpgradeStepsXml.CreateElement(userUpgradeStepElementName);
				element.SetAttribute(identifierAttributeName, identifier);
				CompletedUserUpgradeSteps.AppendChild(element);
			}

			return element;
		}

		public bool IsUserUpgradeStepCompleted(string identifier)
		{
			if (IsSynchronized)
				Monitor.Enter(this);
			
			try
			{
				return null != GetUserUpgradeStepElement(identifier, false);
			}
			finally
			{
				if (IsSynchronized)
					Monitor.Exit(this);
			}
		}

		public void OnUserUpgradeStepCompleted(string identifier)
		{
			if (IsSynchronized)
				Monitor.Enter(this);

			try
			{
				GetUserUpgradeStepElement(identifier, true);
				if (UnitTesting) //don't save
					return;

				//TODO: on application stop?
				Save();
			}
			finally
			{
				if (IsSynchronized)
					Monitor.Exit(this);
			}
		}
	}
}

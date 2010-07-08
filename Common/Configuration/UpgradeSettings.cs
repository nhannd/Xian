#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

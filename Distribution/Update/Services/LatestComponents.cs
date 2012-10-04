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
using System.Xml.Serialization;

namespace ClearCanvas.Distribution.Update.Services
{
	[Serializable]
	public class ComponentInfo
	{
		[XmlAttribute]
		public string Name = "";

		[XmlArrayItem("Edition")]
		public List<EditionInfo> Editions;

		public Component GetUpgradeFor(Component installedComponent, out string downloadUrl)
		{
			installedComponent.Validate();
			downloadUrl = String.Empty;

			if (Editions == null || Editions.Count == 0)
				return null;

			var latestComponent = new Component {Name = Name};
			foreach (var edition in Editions)
			{
				edition.UpdateComponent(latestComponent);
				if (edition.UpgradeRules == null || edition.UpgradeRules.Count == 0)
					continue;

				latestComponent.Validate();
				
				foreach (var upgradeRule in edition.UpgradeRules)
				{
					var test = upgradeRule.Test;
					if (!test.Test(latestComponent, installedComponent))
						continue;

					latestComponent.Version = latestComponent.GetVersion().ToString(2);
					downloadUrl = edition.DownloadUrl;
					if (upgradeRule.ReturnComponentHack != null)
						latestComponent.Name = upgradeRule.ReturnComponentHack;
					if (upgradeRule.ReturnVersionHack != null)
						latestComponent.Version = upgradeRule.ReturnVersionHack;

					return latestComponent;
				}
			}

			return null;
		}
	}

	[Serializable]
	public class EditionInfo
	{
		[XmlAttribute]
		public string Name = "";
		[XmlAttribute]
		public string Version = "";
		[XmlAttribute]
		public string VersionSuffix = "";
		
		//for now, we don't put anything but official releases on the website
		//[XmlAttribute]
		//public string Release = "";
		[XmlAttribute]
		public string DownloadUrl = "";

		public List<UpgradeRule> UpgradeRules;

		internal void UpdateComponent(Component component)
		{
			component.Edition = Name;
			component.Version = Version;
			component.VersionSuffix = VersionSuffix;
			//component.Release = Release;
		}
	}

	[Serializable]
	public class UpgradeRule
	{
		public ComponentUpgradeTest Test;
		//later, could add some more information.
		public string ReturnComponentHack = null;
		public string ReturnVersionHack = null;
	}

	[Serializable]
	public class LatestComponents
	{
		[XmlArrayItem("Component")]
		public List<ComponentInfo> Components;

		internal Component GetUpgradeFor(Component component, out string downloadUrl)
		{
			downloadUrl = String.Empty;
			if (Components != null && Components.Count > 0)
			{
				foreach (ComponentInfo latestComponent in Components)
				{
					var upgrade = latestComponent.GetUpgradeFor(component, out downloadUrl);
					if (upgrade == null)
						continue;

					if (Logger.IsDebugEnabled)
						Logger.DebugFormat("Upgrade for '{0}' is '{1}'", component, upgrade);

					return upgrade;
				}
			}

			if (Logger.IsDebugEnabled)
				Logger.DebugFormat("No upgrade found for '{0}'", component);

			return Component.Empty;
		}

		internal static LatestComponents CreateCurrent()
		{
			var componentInfo = new ComponentInfo
			{
				Name = ComponentNames.Workstation,
				Editions = new List<EditionInfo>(new[] { CreateLatestWorkstationCommunityEdition(), CreateLatestClinicalEdition() })
			};

			return new LatestComponents { Components = new List<ComponentInfo>(new[] { componentInfo }) };
		}

		internal static EditionInfo CreateLatestWorkstationCommunityEdition()
		{
			return new EditionInfo
			{
				Name = EditionNames.Community,
				Version = WorkstationVersions.v20SP1,
				VersionSuffix = VersionSuffixes.SP1,
				DownloadUrl = "http://www.google.ca",
				//Clinical is the upgrade for all community editions, so this is an upgrade for nothing at the moment.
				UpgradeRules = new List<UpgradeRule> { new UpgradeRule { Test = new FalseTest() } }
			};
		}

		internal static EditionInfo CreateLatestClinicalEdition()
		{
			var legacyRule = new UpgradeRule
			{
				Test = new AndTest
				{
					InnerTests = new List<ComponentUpgradeTest>
	       		        				{
	       		        					new IsSameComponentTest(),
	       		        					new IsCommunityEditionTest { Target = TestTarget.Installed },
	       		        					new IsLatestVersionNewerTest()
	       		        				}
				},
				//Hack to make 1.5 and 2.0 display "Clinical".
				ReturnVersionHack = "3.0 " + EditionNames.Clinical
			};

			var standardRule = new UpgradeRule { Test = new StandardUpgradeTest() };

			return new EditionInfo
			{
				Name = EditionNames.Clinical,
				Version = WorkstationVersions.v30ClinicalOfficial,
				DownloadUrl = DownloadUrls.Default,
				UpgradeRules = new List<UpgradeRule> { legacyRule, standardRule }
			};
		}
	}
}
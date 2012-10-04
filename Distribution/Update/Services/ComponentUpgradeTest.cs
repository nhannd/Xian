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
	public class IsSameComponentTest : ComponentUpgradeTest
	{
		public override bool Test(Component latestComponent, Component installedComponent)
		{
			return installedComponent.Name == latestComponent.Name;
		}
	}

	[Serializable]
	public class IsSameEditionTest : ComponentUpgradeTest
	{
		public override bool Test(Component latestComponent, Component installedComponent)
		{
			return installedComponent.IsCommunityEdition && latestComponent.IsCommunityEdition ||
				   installedComponent.Edition == latestComponent.Edition;
		}
	}

	[Serializable]
	public class IsCommunityEditionTest : SingleComponentTest
	{
		public override bool Test(Component component)
		{
			return component.IsCommunityEdition;
		}
	}

	[Serializable]
	public class IsSameVersionTest : ComponentUpgradeTest
	{
		public override bool Test(Component latestComponent, Component installedComponent)
		{
			return latestComponent.GetVersion() == installedComponent.GetVersion();
		}
	}

	[Serializable]
	public class IsLatestVersionNewerTest : ComponentUpgradeTest
	{
		public override bool Test(Component latestComponent, Component installedComponent)
		{
			return installedComponent.GetVersion() < latestComponent.GetVersion();
		}
	}

	[Serializable]
	public class IsPropertyEqualTest : SingleComponentTest
	{
		public string Property;
		public string Value;

		public override bool Test(Component component)
		{
			var value = typeof (Component).GetField(Property).GetValue(component);
			return Equals(value, Value);
		}
	}
	
	public enum Comparison
	{
		Equal,
		Less,
		Greater,
		LessEqual,
		GreaterEqual
	}

	[Serializable]
	public class AndTest : CompositeTest
	{
		public override bool Test(Component latestComponent, Component installedComponent)
		{
			if (InnerTests == null || InnerTests.Count == 0)
				return true;

			foreach (var test in InnerTests)
			{
				if (!test.Test(latestComponent, installedComponent))
					return false;
			}

			return true;
		}
	}

	[Serializable]
	public class OrTest : CompositeTest
	{
		public override bool Test(Component latestComponent, Component installedComponent)
		{
			if (InnerTests == null || InnerTests.Count == 0)
				return true;

			foreach (var test in InnerTests)
			{
				if (test.Test(latestComponent, installedComponent))
					return true;
			}

			return false;
		}
	}

	public class NotTest : ComponentUpgradeTest
	{
		public ComponentUpgradeTest InnerTest;

		public override bool Test(Component latestComponent, Component installedComponent)
		{
			if (InnerTest == null)
				return false;

			return !InnerTest.Test(latestComponent, installedComponent);
		}
	}

	public class FalseTest : ComponentUpgradeTest
	{
		public override bool Test(Component latestComponent, Component installedComponent)
		{
			return false;
		}
	}

	public class TrueTest : ComponentUpgradeTest
	{
		public override bool Test(Component latestComponent, Component installedComponent)
		{
			return true;
		}
	}

	[Serializable]
	public abstract class CompositeTest : ComponentUpgradeTest
	{
		[XmlArrayItem("InnerTest")]
		public List<ComponentUpgradeTest> InnerTests;
	}

	[Serializable]
	public class StandardUpgradeTest : ComponentUpgradeTest
	{
		private readonly AndTest _innerTest;

		public StandardUpgradeTest()
		{
			_innerTest = new AndTest
			{
				InnerTests = new List<ComponentUpgradeTest>
			             	{
			             		new IsSameComponentTest(),
			             		new IsSameEditionTest(),
			             		new IsLatestVersionNewerTest()
			             	}
			};
		}

		public override bool Test(Component latestComponent, Component installedComponent)
		{
			return _innerTest.Test(latestComponent, installedComponent);
		}
	}

	public enum TestTarget
	{
		Installed,
		Latest
	}

	public abstract class SingleComponentTest : ComponentUpgradeTest
	{
		[XmlAttribute]
		public TestTarget Target = TestTarget.Installed;

		public abstract bool Test(Component component);

		public sealed override bool Test(Component latestComponent, Component installedComponent)
		{
			return Test(Target == TestTarget.Installed ? installedComponent : latestComponent);
		}
	}

	public abstract class ComponentUpgradeTest
	{
		public abstract bool Test(Component latestComponent, Component installedComponent);
	}
}

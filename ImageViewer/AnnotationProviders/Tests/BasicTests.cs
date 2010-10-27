#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Tests
{
	[TestFixture]
	public class BasicTests
	{
		private List<IAnnotationItemProvider> _existingProviders;

		public BasicTests()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			_existingProviders = new List<IAnnotationItemProvider>();

			object[] types = this.GetType().Assembly.GetTypes();
			foreach (Type type in types)
			{
				object[] attributes = type.GetCustomAttributes(typeof(ExtensionOfAttribute), false);
				foreach (ExtensionOfAttribute extension in attributes)
				{
					if (extension.ExtensionPointClass == typeof(AnnotationItemProviderExtensionPoint))
					{
						IAnnotationItemProvider provider = (IAnnotationItemProvider)Activator.CreateInstance(type);
						_existingProviders.Add(provider);
					}
				}
			}
		}


		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void TestAnnotationItemProviderIdentifier()
		{
			List<string> uniqueIdentifiers = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				string result = provider.GetIdentifier();
				if (uniqueIdentifiers.Contains(result))
					Assert.Fail();

				uniqueIdentifiers.Add(result);

				if (string.IsNullOrEmpty(result))
					Assert.Fail();
			}
		}

		[Test]
		public void TestAnnotationItemProviderDisplayName()
		{
			List<string> uniqueDisplayNames = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				string result = provider.GetDisplayName();
				if (uniqueDisplayNames.Contains(result))
					Assert.Fail();

				uniqueDisplayNames.Add(result);

				if (string.IsNullOrEmpty(result))
					Assert.Fail();
			}
		}

		[Test]
		public void TestAnnotationItemIdentifiers()
		{
			List<string> uniqueIdentifiers = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					string result = item.GetIdentifier();
					if (uniqueIdentifiers.Contains(result))
						Assert.Fail();

					uniqueIdentifiers.Add(result);

					if (string.IsNullOrEmpty(result))
						Assert.Fail();
				}
			}
		}

		[Test]
		public void TestAnnotationItemDisplayNames()
		{
			List<string> uniqueDisplayNames = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					string result = item.GetDisplayName();
					if (uniqueDisplayNames.Contains(result))
						Assert.Fail();

					uniqueDisplayNames.Add(result);

					if (string.IsNullOrEmpty(result))
						Assert.Fail();
				}
			}
		}

		[Test]
		public void TestAnnotationItemNullImage()
		{
			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					string result = item.GetAnnotationText(null);
					if (!string.IsNullOrEmpty(result))
						Assert.Fail();
				}
			}
		}
	}
}

#endif
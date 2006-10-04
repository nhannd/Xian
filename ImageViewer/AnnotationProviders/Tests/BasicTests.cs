#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Tests
{
	/// <summary>
	/// For the most part, what we are testing for here is that the annotation item providers
	/// (and the annotation items they provide) conform to a a set of rules defined by these tests.
	/// 
	/// The rules include:
	/// 
	/// -  Identifier naming conventions loosely based on namespace class/names, without simply using the 
	///    full class name (namespace + class).  The reason for this is that we want the identifer(s)
	///    to be as independent as possible from any code refactoring etc, while still maintaining some
	///    sort of structure as this kind of thing could easily get very messy.
	/// 
	/// -  The number of IAnnotationItems within a (sub) namespace in which there is an IAnnotationItemProvider
	///    extension defined is the same as the number of IAnnotationItems provided by that extension.  This
	///    ensures that when somebody adds a new item, they haven't forgotten to 'provide' it.
	/// 
	/// -  The Identifier and DisplayName of all IAnnotationItemProviders and IAnnotationItems are both
	///    unique and meaningful.  This wards off copy/paste errors.
	/// 
	/// </summary>
	[TestFixture]
	public class BasicTests
	{
	
		private static readonly string _providerIdentifierPrefix = "AnnotationItemProviders";
		private static readonly string _annotationItemIdentifierSuffix = "AnnotationItem";

		private string _assemblyNamespace;
		
		private Dictionary<string, int> _annotationItemsInNamespace;
		private List<IAnnotationItemProvider> _existingProviders;

		public BasicTests()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			_assemblyNamespace = this.GetType().Namespace.Replace("Test", "");

			_annotationItemsInNamespace = new Dictionary<string, int>();

			_existingProviders = new List<IAnnotationItemProvider>();

			object[] types = this.GetType().Assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.GetInterface(typeof(IAnnotationItem).Name) != null)
				{
					if (!_annotationItemsInNamespace.ContainsKey(type.Namespace))
						_annotationItemsInNamespace.Add(type.Namespace, 1);
					else
						++_annotationItemsInNamespace[type.Namespace];

					continue;
				}

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
		public void TestAnnotationItemProviderNamingConventions()
		{
			string removePrefix = _assemblyNamespace + ".";

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				Type providerType = provider.GetType();
				
				string name = providerType.Namespace.Replace(removePrefix, "");
				name = _providerIdentifierPrefix + "." + name;

				if (name != provider.GetIdentifier())
					Assert.Fail("Provider identifier doesn't conform to naming conventions.");
			}
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

				if (result == SR.Unknown)
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

				if (result == SR.Unknown)
					Assert.Fail();
			}
		}

		[Test]
		public void TestAnnotationItemProviderAnnotationItems()
		{
			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				IEnumerable<IAnnotationItem> items = provider.GetAnnotationItems();
				if (items == null)
					Assert.Fail();

				int itemsProvided = 0;
				foreach (IAnnotationItem item in items)
					++itemsProvided;

				if (_annotationItemsInNamespace[provider.GetType().Namespace] != itemsProvided)
					Assert.Fail();
			}
		}

		[Test]
		public void TestAnnotationItemNamingConventions()
		{
			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				//Send up a red flag if the naming conventions are not enforced.
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					Type type = item.GetType();
					string removePrefix = _assemblyNamespace + ".";

					string name = type.FullName.Replace(removePrefix, "");
					name = name.Replace(_annotationItemIdentifierSuffix, "");

					if (name != item.GetIdentifier())
						Assert.Fail("Item identifier doesn't conform to naming conventions.");
				}
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

					if (result == SR.Unknown)
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

					if (result == SR.Unknown)
						Assert.Fail();
				}
			}
		}

		[Test]
		public void TestAnnotationItemLabels()
		{
			List<string> uniqueLabels = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					string result = item.GetLabel();
					if (uniqueLabels.Contains(result))
						Assert.Fail();

					uniqueLabels.Add(result);

					if (string.IsNullOrEmpty(result))
						Assert.Fail();

					if (result == SR.Unknown)
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
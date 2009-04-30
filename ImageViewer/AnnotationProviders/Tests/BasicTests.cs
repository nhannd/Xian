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
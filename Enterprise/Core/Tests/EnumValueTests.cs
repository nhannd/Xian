#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

#pragma warning disable 1591

using NUnit.Framework;

namespace ClearCanvas.Enterprise.Core.Tests
{
	[TestFixture]
	public class EnumValueTests
	{
		public class ConcreteEnumValue : EnumValue
		{
			/// <summary>
			/// Constructor required for dynamic proxy.
			/// </summary>
			public ConcreteEnumValue()
			{
			}

			public ConcreteEnumValue(string code)
				:base(code, null, null)
			{
			}
		}

		public class ConcreteEnumValue2 : EnumValue
		{
			/// <summary>
			/// Constructor required for dynamic proxy.
			/// </summary>
			public ConcreteEnumValue2()
			{
			}

			public ConcreteEnumValue2(string code)
				: base(code, null, null)
			{
			}
		}


		[Test]
		public void Test_CreateProxy()
		{
			var raw = new ConcreteEnumValue("a");
			var proxy = EntityProxyFactory.CreateProxy(raw);

			// the proxy and raw instance are not the same
			Assert.IsFalse(ReferenceEquals(raw, proxy));
		}

		[Test]
		public void Test_GetClass_returns_type_of_raw_instance()
		{
			var raw = new ConcreteEnumValue("a");
			var proxy = EntityProxyFactory.CreateProxy(raw);

			// the type of the proxy is not the type of the raw instance
			Assert.AreNotEqual(typeof(ConcreteEnumValue), proxy.GetType());
			
			// the GetClass method returns the type of the raw instance
			Assert.AreEqual(typeof(ConcreteEnumValue), proxy.GetClass());
		}

		[Test]
		public void Test_Equals_correctly_compares_proxy_and_raw_instances()
		{
			var raw = new ConcreteEnumValue("a");
			var proxy = EntityProxyFactory.CreateProxy(raw);

			// the proxy and raw instance are not the same
			Assert.IsFalse(ReferenceEquals(raw, proxy));

			// check every possible permutation
			Assert.IsTrue(raw.Equals(raw));
			Assert.IsTrue(proxy.Equals(proxy));
			Assert.IsTrue(raw.Equals(proxy));
			Assert.IsTrue(proxy.Equals(raw));
		}

		[Test]
		public void Test_GetHashCode_identical_between_proxy_and_raw_instances()
		{
			var raw = new ConcreteEnumValue("a");
			var proxy = EntityProxyFactory.CreateProxy(raw);

			// the proxy and raw instance are not the same
			Assert.IsFalse(ReferenceEquals(raw, proxy));

			var x = raw.GetHashCode();
			var y = proxy.GetHashCode();

			// hash codes are same
			Assert.AreEqual(x, y);
		}

		[Test]
		public void Test_Equals_fails_for_same_code_different_class()
		{
			var a1 = new ConcreteEnumValue("a");
			var a2 = new ConcreteEnumValue2("a");

			Assert.AreNotEqual(a1, a2);
		}
	}
}

#endif

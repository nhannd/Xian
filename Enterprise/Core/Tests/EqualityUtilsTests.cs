#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	public class EqualityUtilsTests
	{
		public class ConcreteEntity : Entity
		{
			
		}

		public class ConcreteEnumValue : EnumValue
		{
			/// <summary>
			/// Constructor required for dynamic proxy.
			/// </summary>
			public ConcreteEnumValue()
			{
			}

			public ConcreteEnumValue(string code)
				: base(code, null, null)
			{
			}
		}

		[Test]
		public void Test_AreEqual_compare_distinct_entities()
		{
			var e1 = new ConcreteEntity();
			var e2 = new ConcreteEntity();

			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(e1, e1));
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(e2, e2));
			Assert.IsFalse(EqualityUtils<ConcreteEntity>.AreEqual(e1, e2));
			Assert.IsFalse(EqualityUtils<ConcreteEntity>.AreEqual(e2, e1));

			// also works using Entity as generic arg
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(e1, e1));
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(e2, e2));
			Assert.IsFalse(EqualityUtils<Entity>.AreEqual(e1, e2));
			Assert.IsFalse(EqualityUtils<Entity>.AreEqual(e2, e1));
		}

		[Test]
		public void Test_AreEqual_compare_entity_and_proxy()
		{
			var e1 = new ConcreteEntity();
			var e2 = EntityProxyFactory.CreateProxy(e1);

			// all permutations should be equal
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(e1, e1));
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(e2, e2));
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(e1, e2));
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(e2, e1));

			// also works using Entity as generic arg
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(e1, e1));
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(e2, e2));
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(e1, e2));
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(e2, e1));
		}

		[Test]
		public void Test_AreEqual_compare_entity_and_null()
		{
			var e1 = new ConcreteEntity();

			Assert.IsFalse(EqualityUtils<ConcreteEntity>.AreEqual(e1, null));
			Assert.IsFalse(EqualityUtils<ConcreteEntity>.AreEqual(null, e1));
			Assert.IsFalse(EqualityUtils<Entity>.AreEqual(e1, null));
			Assert.IsFalse(EqualityUtils<Entity>.AreEqual(null, e1));
		}

		[Test]
		public void Test_AreEqual_does_not_initialize_Entity_proxy_if_not_needed()
		{
			var raw = new ConcreteEntity();
			EntityProxyFactory.EntityProxyInterceptor interceptor;
			var proxy = EntityProxyFactory.CreateProxy(raw, out interceptor);

			// check equality between proxies
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(proxy, proxy));
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(proxy, proxy));

			// ensure interceptor did not intercept anything (ie initialize proxy)
			Assert.IsFalse(interceptor.Intercepted);

			// check equality between proxy and raw
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(raw, proxy));

			// in this case, interceptor is invoked
			Assert.IsTrue(interceptor.Intercepted);
		}

		[Test]
		public void Test_AreEqual_compare_distinct_EnumValues()
		{
			var e1 = new ConcreteEnumValue("1");
			var e2 = new ConcreteEnumValue("2");

			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(e1, e1));
			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(e2, e2));
			Assert.IsFalse(EqualityUtils<ConcreteEnumValue>.AreEqual(e1, e2));
			Assert.IsFalse(EqualityUtils<ConcreteEnumValue>.AreEqual(e2, e1));

			// also works using EnumValue as generic arg
			Assert.IsTrue(EqualityUtils<EnumValue>.AreEqual(e1, e1));
			Assert.IsTrue(EqualityUtils<EnumValue>.AreEqual(e2, e2));
			Assert.IsFalse(EqualityUtils<EnumValue>.AreEqual(e1, e2));
			Assert.IsFalse(EqualityUtils<EnumValue>.AreEqual(e2, e1));
		}

		[Test]
		public void Test_AreEqual_compare_EnumValue_and_proxy()
		{
			var e1 = new ConcreteEnumValue("1");
			var e2 = EntityProxyFactory.CreateProxy(e1);

			// all permutations should be equal
			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(e1, e1));
			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(e2, e2));
			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(e1, e2));
			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(e2, e1));

			// also works using Entity as generic arg
			Assert.IsTrue(EqualityUtils<EnumValue>.AreEqual(e1, e1));
			Assert.IsTrue(EqualityUtils<EnumValue>.AreEqual(e2, e2));
			Assert.IsTrue(EqualityUtils<EnumValue>.AreEqual(e1, e2));
			Assert.IsTrue(EqualityUtils<EnumValue>.AreEqual(e2, e1));
		}

		[Test]
		public void Test_AreEqual_compare_EnumValue_and_null()
		{
			var e1 = new ConcreteEnumValue("1");

			Assert.IsFalse(EqualityUtils<ConcreteEnumValue>.AreEqual(e1, null));
			Assert.IsFalse(EqualityUtils<ConcreteEnumValue>.AreEqual(null, e1));
			Assert.IsFalse(EqualityUtils<EnumValue>.AreEqual(e1, null));
			Assert.IsFalse(EqualityUtils<EnumValue>.AreEqual(null, e1));
		}

		[Test]
		public void Test_AreEqual_does_not_initialize_EnumValue_proxy_if_not_needed()
		{
			var raw = new ConcreteEnumValue("1");
			EntityProxyFactory.EntityProxyInterceptor interceptor;
			var proxy = EntityProxyFactory.CreateProxy(raw, out interceptor);

			// check equality between proxies
			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(proxy, proxy));
			Assert.IsTrue(EqualityUtils<EnumValue>.AreEqual(proxy, proxy));

			// ensure interceptor did not intercept anything (ie initialize proxy)
			Assert.IsFalse(interceptor.Intercepted);

			// check equality between proxy and raw
			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(raw, proxy));

			// in this case, interceptor is invoked
			Assert.IsTrue(interceptor.Intercepted);
		}

		[Test]
		public void Test_AreEqual_compare_nulls()
		{
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(null, null));
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(null, null));
			Assert.IsTrue(EqualityUtils<ConcreteEnumValue>.AreEqual(null, null));
			Assert.IsTrue(EqualityUtils<EnumValue>.AreEqual(null, null));
		}

	}
}

#endif

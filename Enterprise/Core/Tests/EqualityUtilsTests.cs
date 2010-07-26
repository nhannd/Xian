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
		public void Test_AreEqual_compare_nulls()
		{
			Assert.IsTrue(EqualityUtils<ConcreteEntity>.AreEqual(null, null));
			Assert.IsTrue(EqualityUtils<Entity>.AreEqual(null, null));
		}

		[Test]
		public void Test_AreEqual_does_not_initialize_proxy_if_not_needed()
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
	}
}

#endif

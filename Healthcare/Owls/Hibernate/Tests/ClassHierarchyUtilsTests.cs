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

using System.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Tests
{
	[TestFixture]
	public class ClassHierarchyUtilsTests
	{
		/*				A
		 *			   / \
		 *			  B   C
		 *			 / \   \
		 *			D   E   F	
		 * 
		 */


		class A {}
		class B: A {}
		class C: A {}
		class D: B {}
		class E: B {}
		class F: C {}





		[Test]
		public void Test_GetAllTypesInHierarchy_WithTypicalInput()
		{
			var allTypes = new List<Type> { typeof (A), typeof (B), typeof (C), typeof (D), typeof (E), typeof (F)};
			var concreteTypes = new List<Type> { typeof(D), typeof(E), typeof(F) };

			var result = ClassHierarchyUtils.GetAllTypesInHierarchy(concreteTypes);
			Assert.AreEqual(allTypes.Count, result.Count);

			AssertSetsAreEqual(allTypes, result);
		}

		[Test]
		public void Test_GetAllTypesInHierarchy_WithEmptyInput()
		{
			var result = ClassHierarchyUtils.GetAllTypesInHierarchy(new List<Type>());
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void Test_GetAllTypesInHierarchy_WithSingleRootClassInput()
		{
			var result = ClassHierarchyUtils.GetAllTypesInHierarchy(new[]{typeof(A)});
			AssertSetsAreEqual(new[]{typeof(A)}, result);
		}

		[Test]
		public void Test_GetAllTypesInHierarchy_WithSingleLevel2ClassInput()
		{
			var allTypes = new List<Type> { typeof(A), typeof(B) };
			var concreteTypes = new List<Type> { typeof(B) };

			var result = ClassHierarchyUtils.GetAllTypesInHierarchy(concreteTypes);
			AssertSetsAreEqual(allTypes, result);
		}

		[Test]
		public void Test_GetAllTypesInHierarchy_WithSingleLevel3ClassInput()
		{
			var allTypes = new List<Type> { typeof(A), typeof(B), typeof(E) };
			var concreteTypes = new List<Type> { typeof(E) };

			var result = ClassHierarchyUtils.GetAllTypesInHierarchy(concreteTypes);
			AssertSetsAreEqual(allTypes, result);
		}

		[Test]
		public void Test_GetAllTypesInHierarchy_WithRedundantInput()
		{
			var allTypes = new List<Type> { typeof(A), typeof(B), typeof(E) };
			var concreteTypes = new List<Type> { typeof(E), typeof(B) };	// B is redundant in this list

			var result = ClassHierarchyUtils.GetAllTypesInHierarchy(concreteTypes);
			AssertSetsAreEqual(allTypes, result);
		}

		[Test]
		public void Test_GetClassHierarchyMap_WithTypicalInput()
		{
			var allTypes = new List<Type> { typeof(A), typeof(B), typeof(C), typeof(D), typeof(E), typeof(F) };
			var concreteTypes = new List<Type> { typeof(D), typeof(E), typeof(F) };

			var result = ClassHierarchyUtils.GetClassHierarchyMap(concreteTypes);
			Assert.AreEqual(allTypes.Count, result.Count);
			AssertSetsAreEqual(new[] { typeof(D), typeof(E), typeof(F) }, result[typeof(A)]);
			AssertSetsAreEqual(new[] { typeof(D), typeof(E) }, result[typeof(B)]);
			AssertSetsAreEqual(new[] { typeof(F) }, result[typeof(C)]);
			AssertSetsAreEqual(new[] { typeof(D) }, result[typeof(D)]);
			AssertSetsAreEqual(new[] { typeof(E) }, result[typeof(E)]);
			AssertSetsAreEqual(new[] { typeof(F) }, result[typeof(F)]);
		}

		[Test]
		public void Test_GetClassHierarchyMap_WithEmptyInput()
		{
			var result = ClassHierarchyUtils.GetClassHierarchyMap(new Type[0]);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void Test_GetClassHierarchyMap_WithSingleRootClassInput()
		{
			var result = ClassHierarchyUtils.GetClassHierarchyMap(new[] { typeof(A) });
			Assert.AreEqual(1, result.Count);
			AssertSetsAreEqual(new[] { typeof(A) }, result[typeof(A)]);
		}

		[Test]
		public void Test_GetClassHierarchyMap_WithSingleLevel2ClassInput()
		{
			var result = ClassHierarchyUtils.GetClassHierarchyMap(new[] { typeof(B) });
			Assert.AreEqual(2, result.Count);
			AssertSetsAreEqual(new[] { typeof(B) }, result[typeof(A)]);
			AssertSetsAreEqual(new[] { typeof(B) }, result[typeof(B)]);
		}

		[Test]
		public void Test_GetClassHierarchyMap_WithSingleLevel3ClassInput()
		{
			var result = ClassHierarchyUtils.GetClassHierarchyMap(new[] { typeof(E) });
			Assert.AreEqual(3, result.Count);
			AssertSetsAreEqual(new[] { typeof(E) }, result[typeof(A)]);
			AssertSetsAreEqual(new[] { typeof(E) }, result[typeof(B)]);
			AssertSetsAreEqual(new[] { typeof(E) }, result[typeof(E)]);
		}

		[Test]
		public void Test_GetClassHierarchyMap_WithRedundantInput()
		{
			var concreteTypes = new List<Type> { typeof(E), typeof(B) };	// B is redundant in this list

			var result = ClassHierarchyUtils.GetClassHierarchyMap(concreteTypes);
			Assert.AreEqual(3, result.Count);
			AssertSetsAreEqual(new[] { typeof(E), typeof(B) }, result[typeof(A)]);
			AssertSetsAreEqual(new[] { typeof(E), typeof(B) }, result[typeof(B)]);
			AssertSetsAreEqual(new[] { typeof(E) }, result[typeof(E)]);
		}

		private static void AssertSetsAreEqual(IList expected, IList result)
		{
			Assert.AreEqual(expected.Count, result.Count);
			foreach (var type in expected)
			{
				Assert.Contains(type, result);
			}
		}

	}
}

#endif

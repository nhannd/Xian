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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls.Hibernate
{
	/// <summary>
	/// Utility class for working with class hierarchies.
	/// </summary>
	internal static class ClassHierarchyUtils
	{
		/// <summary>
		/// Given a set of concrete types, returns a dictionary containing exactly one entry
		/// for every type in the hierarchy, where the value is a list containing the concrete
		/// types that inherit from that type.
		/// </summary>
		/// <param name="concreteTypes"></param>
		/// <returns></returns>
		internal static Dictionary<Type, List<Type>> GetClassHierarchyMap(IList<Type> concreteTypes)
		{
			var result = new Dictionary<Type, List<Type>>();
			var allTypes = GetAllTypesInHierarchy(concreteTypes);
			foreach (var type in allTypes)
			{
				result.Add(type, CollectionUtils.Select(concreteTypes, t => type.IsAssignableFrom(t)));
			}
			return result;
		}

		/// <summary>
		/// Given a set of concrete types, returns a list of all types in the hierarchy(s).
		/// </summary>
		/// <param name="concreteTypes"></param>
		/// <returns></returns>
		internal static List<Type> GetAllTypesInHierarchy(IList<Type> concreteTypes)
		{
			var results = new List<Type>();
			for (var i = 0; i < concreteTypes.Count; i++)
			{
				var type = concreteTypes[i];
				while(type != typeof(object))
				{
					results.Add(type);
					type = type.BaseType;
				}
			}
			return CollectionUtils.Unique(results);
		}
	}
}

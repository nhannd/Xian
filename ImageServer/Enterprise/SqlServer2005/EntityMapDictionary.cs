#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
	public static class EntityMapDictionary
	{
		private static readonly object _syncLock = new object();
		private static Dictionary<Type, Dictionary<string, PropertyInfo>> _maps = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

		public static Dictionary<string, PropertyInfo> GetEntityMap(Type entityType)
		{
			lock (_syncLock)
			{
				if (_maps.ContainsKey(entityType))
					return _maps[entityType];

				Dictionary<string, PropertyInfo> propMap = LoadMap(entityType);
				_maps.Add(entityType, propMap);
				return propMap;
			}
		}

		private static Dictionary<string, PropertyInfo> LoadMap(Type entityType)
		{
			ObjectWalker walker = new ObjectWalker();
			Dictionary<string, PropertyInfo> propMap = new Dictionary<string, PropertyInfo>();

			foreach (IObjectMemberContext member in walker.Walk(entityType))
			{
				EntityFieldDatabaseMappingAttribute map =
					AttributeUtils.GetAttribute<EntityFieldDatabaseMappingAttribute>(member.Member);
				if (map != null)
				{
					propMap.Add(map.ColumnName, member.Member as PropertyInfo);
				}
			}

			return propMap;
		}
	}
}

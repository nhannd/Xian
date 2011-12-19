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
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    public static class EntityMapDictionary
    {
        private static readonly object SyncLock = new object();
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> Maps = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public static Dictionary<string, PropertyInfo> GetEntityMap(Type entityType)
        {
            lock (SyncLock)
            {
                if (Maps.ContainsKey(entityType))
                    return Maps[entityType];

                Dictionary<string, PropertyInfo> propMap = LoadMap(entityType);
                Maps.Add(entityType, propMap);
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
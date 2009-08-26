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

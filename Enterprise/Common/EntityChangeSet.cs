#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Common
{
	public class EntityChangeSet
	{
		private readonly List<EntityChange> _changes;

		public EntityChangeSet(IEnumerable<EntityChange> changes)
		{
			_changes = new List<EntityChange>(changes);
		}

		public IList<EntityChange> Changes
		{
			get { return _changes.AsReadOnly(); }
		}
	}
}

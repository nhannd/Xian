#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class AddMetadata : Task
	{
		[Output]
		[Required]
		public ITaskItem[] Items { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string[] Values { get; set; }

		public override bool Execute()
		{
			if (IsNullOrEmpty(Items))
			{
				Log.LogError("Items must be supplied.");
				return false;
			}

			if (IsNullOrEmpty(Values))
			{
				Log.LogError("Values must be supplied.");
				return false;
			}

			if (Items.Length != Values.Length)
			{
				Log.LogError("Items and Values must have the same number of items.");
				return false;
			}

			var newItems = new ITaskItem[Items.Length];
			for (int n = 0; n < Items.Length; n++)
			{
				var newItem = newItems[n] = new TaskItem(Items[n]);
				newItem.SetMetadata(Name, Values[n]);
			}
			Items = newItems;
			return true;
		}

		private static bool IsNullOrEmpty<T>(ICollection<T> collection)
		{
			return collection == null || collection.Count == 0;
		}
	}
}
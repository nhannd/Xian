#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class SplitString : Task
	{
		public string Separators { get; set; }
		public string Input { get; set; }

		[Output]
		public string[] Items { get; set; }

		public override bool Execute()
		{
			if (!string.IsNullOrEmpty(Input))
				Items = Input.Split(Separators.ToCharArray());
			return true;
		}
	}
}
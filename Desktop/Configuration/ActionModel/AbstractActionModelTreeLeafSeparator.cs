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
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public sealed class AbstractActionModelTreeLeafSeparator : AbstractActionModelTreeLeaf
	{
		public AbstractActionModelTreeLeafSeparator() : base(new PathSegment("Separator", SR.LabelSeparator)) 
		{
			base.IconSet = new IconSet("Icons.ActionModelSeparatorSmall.png", "Icons.ActionModelSeparatorMedium.png", "Icons.ActionModelSeparatorLarge.png");
			base.ResourceResolver = new ApplicationThemeResourceResolver(this.GetType().Assembly);
			base.CheckState = CheckState.Checked;
		}

		internal Path GetSeparatorPath()
		{
			Stack<PathSegment> stack = new Stack<PathSegment>();
			stack.Push(new PathSegment(Guid.NewGuid().ToString()));

			AbstractActionModelTreeNode current = this.Parent;
			while (current != null)
			{
				stack.Push(current.PathSegment);
				current = current.Parent;
			}

			Path path = new Path(stack.Pop());
			while (stack.Count > 0)
			{
				path = path.Append(stack.Pop());
			}
			return path;
		}
	}
}
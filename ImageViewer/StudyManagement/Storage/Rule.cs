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
using System.Linq;
using System.Text;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	partial class Rule
	{
		public RuleData RuleData
		{
			get
			{
				return Serializer.DeserializeRule(this.SerializedRule);
			}
			set
			{
				this.SerializedRule = Serializer.SerializeRule(value);
			}
		}
	}
}

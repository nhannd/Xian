#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.Common.Entities
{
	[DataContract(Namespace = ViewerNamespace.Value)]
	public class Cursor
	{
		public Image Icon { get; set; }

		[DataMember(IsRequired = false)]
		public Position HotSpot { get; set; }

		public override string ToString()
		{
			if (Icon == null)
				return "Cursor {null}";

			return String.Format("Cursor {{with icon}} (HotSpot={0})", HotSpot);
		}
	}
}

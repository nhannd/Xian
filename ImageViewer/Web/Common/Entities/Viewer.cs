#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.Common.Entities
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class Viewer : Entity
    {
		[DataMember(IsRequired = false)]
		public WebActionNode[] ToolbarActions { get; set; }

        [DataMember(IsRequired = false)]
        public WebIconSize ToolStripIconSize { get; set; }

		[DataMember(IsRequired = true)]
		public ImageBox[] ImageBoxes { get; set; }

        [DataMember(IsRequired = false)]
        public Entity[] Extensions { get; set; }
	}
}
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
    public class Tile : Entity
    {
		[DataMember(IsRequired = true)]
		public RectangleF NormalizedRectangle { get; set; }

		[DataMember(IsRequired = false)]
		public Rectangle ClientRectangle { get; set; }

		[DataMember(IsRequired = false)]
		public bool Selected { get; set; }

		[DataMember(IsRequired = false)]
		public bool HasCapture { get; set; }

        [DataMember(IsRequired = false)]
        public bool HasWheelCapture { get; set; }

        [DataMember(IsRequired = false)]
        public Position MousePosition { get; set; }

        [DataMember(IsRequired = false)]
        public InformationBox InformationBox { get; set; }

        [DataMember(IsRequired = false)]
        public Image Image { get; set; }
        
        [DataMember(IsRequired = false)]
		public Cursor Cursor { get; set; }
    }
}
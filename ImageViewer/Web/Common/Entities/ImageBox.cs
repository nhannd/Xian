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
using System;

namespace ClearCanvas.ImageViewer.Web.Common.Entities
{
    [JavascriptModule("ClearCanvas/Controllers/ImageViewer/ImageBoxController")]
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class ImageBox : Entity
    {
		[DataMember(IsRequired = true)]
		public RectangleF NormalizedRectangle { get; set; }
		
		[DataMember(IsRequired = true)]
        public Tile[] Tiles { get; set; }

		[DataMember(IsRequired = true)]
		public bool Selected { get; set; }

		[DataMember(IsRequired = true)]
        public int ImageCount { get; set; }

		[DataMember(IsRequired = true)]
        public int TopLeftPresentationImageIndex { get; set; }

        [DataMember(IsRequired = true)]
        public Entity Overlay { get; set; }

		public override string ToString()
		{
			return String.Format("{0} [Tiles{{Count={1}}}, NormalizedRectangle={2}]", 
									base.ToString(), Tiles == null ? 0 : Tiles.Length, NormalizedRectangle);
		}
    }
}
#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	internal class DynamicTeAnnotationItem : IAnnotationItem
	{
		public DynamicTeAnnotationItem(IAnnotationItemProvider ownerProvider)
		{ 
		
		}

		#region IAnnotationItem Members

		public string GetIdentifier()
		{
			return "Presentation.DynamicTe";
		}

		public string GetDisplayName()
		{
			return "DynamicTe";
		}

		public string GetLabel()
		{
			return "DynamicTe";
		}

		public string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			IDynamicTeProvider teProvider = presentationImage as IDynamicTeProvider;

			if (teProvider == null)
				return string.Empty;

			string annotationText = String.Format("Echo Time: {0:F2}", teProvider.DynamicTe.Te);

			return annotationText;
		}

		#endregion
	}
}

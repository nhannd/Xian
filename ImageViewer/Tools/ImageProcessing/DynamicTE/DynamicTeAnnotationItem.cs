using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.AnnotationProviders;

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

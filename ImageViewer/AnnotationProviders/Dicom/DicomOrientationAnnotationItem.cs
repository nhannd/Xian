using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal abstract class DicomOrientationAnnotationItem : DicomDoubleArrayAnnotationItem
	{
		public DicomOrientationAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(double[] arrayDoubles)
		{
			//!! Need to decide what to do with this type.
			return base.GetFinalString(arrayDoubles);
		}
	}
}

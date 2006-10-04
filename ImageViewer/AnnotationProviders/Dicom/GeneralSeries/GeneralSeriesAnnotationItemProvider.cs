using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralSeries
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralSeriesAnnotationItemProvider : AnnotationItemProvider
	{
		public GeneralSeriesAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralSeries")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add((IAnnotationItem)new BodyPartExaminedAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new LateralityAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ModalityAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new OperatorsNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientPositionAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PerformedProcedureStepDescriptionAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PerformingPhysiciansNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ProtocolNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new SeriesDateAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new SeriesDescriptionAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new SeriesNumberAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new SeriesTimeAnnotationItem(this));

				return annotationItems;
			}
		}
	}
}

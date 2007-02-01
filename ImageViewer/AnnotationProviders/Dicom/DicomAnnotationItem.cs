using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	public delegate T SopDataRetrieverDelegate<T>(ImageSop imageSop);

	public class DicomAnnotationItem <T>: ResourceResolvingAnnotationItem
	{
		private SopDataRetrieverDelegate<T> _sopDataRetrieverDelegate;
		private ResultFormatterDelegate<T> _resultFormatterDelegate;

		public DicomAnnotationItem
			(
				string identifier,
				IAnnotationItemProvider ownerProvider,
				SopDataRetrieverDelegate<T> sopDataRetrieverDelegate,
				ResultFormatterDelegate<T> resultFormatterDelegate
			)
			: this(identifier, ownerProvider, sopDataRetrieverDelegate, resultFormatterDelegate, false)
		{
		}

		public DicomAnnotationItem
			(
				string identifier,
				IAnnotationItemProvider ownerProvider,
				SopDataRetrieverDelegate<T> sopDataRetrieverDelegate,
				ResultFormatterDelegate<T> resultFormatterDelegate,
				bool allowEmptyLabel
			)
			: this(identifier, ownerProvider, sopDataRetrieverDelegate, resultFormatterDelegate, allowEmptyLabel, null)
		{
		}

		public DicomAnnotationItem
			(
				string identifier,
				IAnnotationItemProvider ownerProvider,
				SopDataRetrieverDelegate<T> sopDataRetrieverDelegate,
				ResultFormatterDelegate<T> resultFormatterDelegate,
				bool allowEmptyLabel,
				IAnnotationResourceResolver resolver
			)
			: base(identifier, ownerProvider, allowEmptyLabel, resolver)
		{
			Platform.CheckForNullReference(sopDataRetrieverDelegate, "sopDataRetrieverDelegate");
			Platform.CheckForNullReference(resultFormatterDelegate, "resultFormatterDelegate");

			_sopDataRetrieverDelegate = sopDataRetrieverDelegate;
			_resultFormatterDelegate = resultFormatterDelegate;
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			IImageSopProvider associatedDicom = presentationImage as IImageSopProvider;
			if (associatedDicom == null)
				return "";

			return _resultFormatterDelegate(_sopDataRetrieverDelegate(associatedDicom.ImageSop));
		}
	}
}

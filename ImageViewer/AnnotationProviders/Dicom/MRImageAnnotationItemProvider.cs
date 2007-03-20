using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class MRImageAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public MRImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.MRImage")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				if (_annotationItems == null)
				{
					_annotationItems = new List<IAnnotationItem>();

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.MRImage.EchoTime",
								this,
								delegate(ImageSop imageSop)
								{
									double val = double.NaN;
									bool tagExists;
									imageSop.GetTag(Dcm.EchoTime, out val, out tagExists);
									string str = String.Format("{0:F2} ms", val);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.MRImage.MagneticFieldStrength",
								this,
								delegate(ImageSop imageSop)
								{
									double val = double.NaN;
									bool tagExists;
									imageSop.GetTag(Dcm.MagneticFieldStrength, out val, out tagExists);

									double strengthInTeslas = val / 10000;
									string str = String.Format("{0:F1}T", strengthInTeslas);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.MRImage.AcquisitionMatrix",
								this,
								delegate(ImageSop imageSop)
								{
									ushort frequencyRows, frequencyColumns, phaseRows, phaseColumns;
									bool tagExists;
									imageSop.GetTag(Dcm.AcquisitionMatrix, out frequencyRows, 0, out tagExists);
									imageSop.GetTag(Dcm.AcquisitionMatrix, out frequencyColumns, 1, out tagExists);
									imageSop.GetTag(Dcm.AcquisitionMatrix, out phaseRows, 2, out tagExists);
									imageSop.GetTag(Dcm.AcquisitionMatrix, out phaseColumns, 3, out tagExists);

									// Just include freq for now
									string str = String.Format("{0} x {1}", frequencyColumns, phaseRows);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.MRImage.ReceiveCoilName",
								this,
								delegate(ImageSop imageSop)
								{
									string val;
									bool tagExists;
									imageSop.GetTag(Dcm.ReceiveCoilName, out val, out tagExists);
									return val;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.MRImage.RepetitionTime",
								this,
								delegate(ImageSop imageSop)
								{
									double val = double.NaN;
									bool tagExists;
									imageSop.GetTag(Dcm.RepetitionTime, out val, out tagExists);
									string str = String.Format("{0:F2} ms", val);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.MRImage.EchoTrainLength",
								this,
								delegate(ImageSop imageSop)
								{
									int val;
									bool tagExists;
									imageSop.GetTag(Dcm.EchoTrainLength, out val, out tagExists);
									string str = String.Format("{0}", val);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);
				}

				return _annotationItems;
			}
		}
	}
}

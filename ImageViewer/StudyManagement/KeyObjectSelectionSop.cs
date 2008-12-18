using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ValueType = ClearCanvas.Dicom.Iod.ValueType;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class KeyObjectSelectionSop : Sop {
		public KeyObjectSelectionSop(ImageSop keyObjectSelectionSop) : base(keyObjectSelectionSop.NativeDicomObject)
		{
			
		}

		internal IList<KeyObjectSop> CreateImages()
		{
			List<KeyObjectSop> list = new List<KeyObjectSop>();

			KeyObjectSelectionDocumentIod kosDocument = new KeyObjectSelectionDocumentIod(base.NativeDicomObject.DataSet);

			int instanceNumber = 1;
			List<IContentSequence> contents = new List<IContentSequence>();
			SrDocumentContentModuleIod srDoc = kosDocument.SrDocumentContent;
			foreach (IContentSequence contentItem in srDoc.ContentSequence) {
				if (contentItem.RelationshipType == RelationshipType.Contains) {
					if (contentItem.ValueType == ValueType.Image) {
						try {
							IImageReferenceMacro imageRef = contentItem;
							list.Add(new KeyObjectSop(this, instanceNumber, imageRef.ReferencedSopSequence.ReferencedSopInstanceUid));
						} catch (Exception ex) {
							Platform.Log(LogLevel.Warn, ex, "Error realizing key object selection content item of value type {0}", contentItem.ValueType);
							continue;
						}
					} else {
						Platform.Log(LogLevel.Warn, "Unsupported key object selection content item of value type {0}", contentItem.ValueType);
						continue;
					}

					instanceNumber++;
				}
			}

			return list.AsReadOnly();
		}
	}
}
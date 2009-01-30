using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ValueType=ClearCanvas.Dicom.Iod.ValueType;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	public class KeyObjectContentItem
	{
		internal KeyObjectContentItem(Frame frame, Sop presentationStateSop)
		{
			this.Frame = frame;
			this.PresentationStateSop = presentationStateSop;
			if (presentationStateSop != null && presentationStateSop.SopClassUID == SopClass.GrayscaleSoftcopyPresentationStateStorageSopClassUid)
				GrayscalePresentationStateIod = new GrayscaleSoftcopyPresentationStateIod(presentationStateSop.DataSource);
		}

		public readonly Frame Frame;
		public readonly Sop PresentationStateSop;

		public readonly GrayscaleSoftcopyPresentationStateIod GrayscalePresentationStateIod;

		//TODO: colour presentation state.
	}

	public class KeyImageDeserializer
	{
		private readonly StudyTree _studyTree;
		private readonly KeyObjectSelectionDocumentIod _document;

		public KeyImageDeserializer(Sop sourceSop, StudyTree studyTree)
		{
			_studyTree = studyTree;
			_document = new KeyObjectSelectionDocumentIod(sourceSop.DataSource);
		}

		public KeyImageDeserializer(KeyObjectSelectionDocumentIod iod, StudyTree studyTree)
		{
			_studyTree = studyTree;
			_document = iod;
		}

		public IList<KeyObjectContentItem> Deserialize()
		{
			List<KeyObjectContentItem> imagePRPairs = new List<KeyObjectContentItem>();

			SrDocumentContentModuleIod srDoc = _document.SrDocumentContent;
			foreach (IContentSequence contentItem in srDoc.ContentSequence)
			{
				if (contentItem.RelationshipType == RelationshipType.Contains)
				{
					if (contentItem.ValueType == ValueType.Image)
					{
						try
						{
							IImageReferenceMacro imageRef = contentItem;
							string referencedSopInstanceUid = imageRef.ReferencedSopSequence.ReferencedSopInstanceUid;
							ImageSop referencedImage = FindReferencedImageSop(referencedSopInstanceUid);
							if (referencedImage == null)
							{
								Platform.Log(LogLevel.Warn, "Unable to find referenced image '{0}'.", referencedSopInstanceUid);
								continue;
							}

							Sop presentationState = null;
							if(imageRef.ReferencedSopSequence.ReferencedSopSequence != null)
							{
								presentationState = FindReferencedSop(imageRef.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopInstanceUid);
							}

							string referencedFrameNumbers = imageRef.ReferencedSopSequence.ReferencedFrameNumber;
							int[] frameNumbers;
							if (DicomStringHelper.TryGetIntArray(referencedFrameNumbers, out frameNumbers) && frameNumbers.Length > 0)
							{
								foreach (int frameNumber in frameNumbers)
								{
									if (frameNumber > referencedImage.Frames.Count)
									{
										Platform.Log(LogLevel.Warn, "Unable to find referenced frame number {0} for instance '{1}'.", frameNumber, referencedSopInstanceUid);
									}
									else
									{
										KeyObjectContentItem item = new KeyObjectContentItem(referencedImage.Frames[frameNumber], presentationState);
										imagePRPairs.Add(item);
									}
								}
							}
							else
							{
								foreach (Frame frame in referencedImage.Frames) {
									KeyObjectContentItem item = new KeyObjectContentItem(frame, presentationState);
									imagePRPairs.Add(item);
								}
							}
						}
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Warn, ex, "Error realizing key object selection content item of value type {0}.", contentItem.ValueType);
							continue;
						}
					}
					else
					{
						Platform.Log(LogLevel.Warn, "Unsupported key object selection content item of value type {0}.", contentItem.ValueType);
						continue;
					}

				}
			}

			return imagePRPairs.AsReadOnly();
		}

		private ImageSop FindReferencedImageSop(string sopInstanceUid)
		{
			string sameStudyUid = _document.GeneralStudy.StudyInstanceUid;
			Study sameStudy = _studyTree.GetStudy(sameStudyUid);
			
			if (sameStudy != null)
			{
				foreach (Series series in sameStudy.Series)
				{
					Sop referencedSop = series.Sops[sopInstanceUid];
					if (referencedSop != null)
						return referencedSop as ImageSop;
				}
			}

			return null;
		}

		private Sop FindReferencedSop(string sopInstanceUid) {
			string sameStudyUid = _document.GeneralStudy.StudyInstanceUid;
			Study sameStudy = _studyTree.GetStudy(sameStudyUid);

			if (sameStudy != null) {
				foreach (Series series in sameStudy.Series) {
					Sop referencedSop = series.Sops[sopInstanceUid];
					if (referencedSop != null)
						return referencedSop;
				}
			}

			return null;
		}
	}
}

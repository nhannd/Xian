using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ValueType=ClearCanvas.Dicom.Iod.ValueType;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	public class KeyObjectDeserializer
	{
		private readonly StudyTree _studyTree;
		private readonly KeyObjectSelectionDocumentIod _document;

		public KeyObjectDeserializer(Sop sourceSop, StudyTree studyTree)
		{
			_studyTree = studyTree;
			_document = new KeyObjectSelectionDocumentIod(sourceSop.DataSource);
		}

		public KeyObjectDeserializer(KeyObjectSelectionDocumentIod iod, StudyTree studyTree)
		{
			_studyTree = studyTree;
			_document = iod;
		}

		public List<IPresentationImage> Deserialize()
		{
			List<IPresentationImage> images = new List<IPresentationImage>();

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
							ImageSop referencedImage = FindReferencedImage(referencedSopInstanceUid);
							if (referencedImage == null)
							{
								Platform.Log(LogLevel.Warn, "Unable to find referenced image '{0}'.", referencedSopInstanceUid);
								continue;
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
										IPresentationImage image = new DicomGrayscalePresentationImage(referencedImage.Frames[frameNumber]);
										images.Add(image);
									}
								}
							}
							else
							{
								foreach (Frame frame in referencedImage.Frames)
								{
									IPresentationImage image = new DicomGrayscalePresentationImage(frame);
									images.Add(image);
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

			return images;
		}

		private ImageSop FindReferencedImage(string sopInstanceUid)
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
	}
}

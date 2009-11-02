using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[ExtensionOf(typeof(ImagePropertyProviderExtensionPoint))]
	public class KeyObjectImagePropertyProvider : IImagePropertyProvider
	{
		public KeyObjectImagePropertyProvider()
		{
		}

		//TODO (cr Oct 2009): discrepancy w/ data?  we show the image data + some key image stuff.
		//TODO (cr Oct 2009): KI Gen Eq: creator, software versions.
		#region IImagePropertyProvider Members

		public IImageProperty[] GetProperties(IPresentationImage image)
		{
			List<IImageProperty> properties = new List<IImageProperty>();

			if (image != null && image.ParentDisplaySet != null)
			{
				IImageViewer viewer = image.ImageViewer;
				if (viewer != null)
				{
					IDicomDisplaySetDescriptor descriptor = image.ParentDisplaySet.Descriptor as IDicomDisplaySetDescriptor;
					if (descriptor != null && descriptor.SourceSeries != null)
					{
						string uid = descriptor.SourceSeries.SeriesInstanceUid;
						if (!String.IsNullOrEmpty(uid))
						{
							StudyTree studyTree = viewer.StudyTree;
							Series keyObjectSeries = studyTree.GetSeries(uid);
							if (keyObjectSeries != null && keyObjectSeries.Sops.Count > 0)
							{
								Sop keyObjectSop = keyObjectSeries.Sops[0];
								if (keyObjectSop.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
								{
									ISopDataSource dataSource = keyObjectSop.DataSource;
									KeyObjectSelectionDocumentIod iod = new KeyObjectSelectionDocumentIod(dataSource);
									SrDocumentContentModuleIod content = iod.SrDocumentContent;
									if (content != null)
									{
										string codeValue = "";
										CodeSequenceMacro conceptSequence = content.ConceptNameCodeSequence;
										if (conceptSequence != null)
										{
											KeyObjectSelectionDocumentTitle documentTitle = KeyObjectSelectionDocumentTitleContextGroup.Lookup(conceptSequence.CodeValue);
											if (documentTitle != null)
												codeValue = documentTitle.ToString();
										}

										string documentDescription = "";
										IContentSequence[] contentSequences = content.ContentSequence ?? new IContentSequence[0];
										for (int i = contentSequences.Length - 1; i >= 0; --i)
										{
											IContentSequence contentSequence = contentSequences[i];
											CodeSequenceMacro sequenceMacro = contentSequence.ConceptNameCodeSequence;
											if (sequenceMacro != null && sequenceMacro.CodeValue == KeyObjectSelectionCodeSequences.KeyObjectDescription.CodeValue)
											{
												documentDescription = contentSequence.TextValue;
												break;
											}
										}

										properties.Add(
											new ImageProperty(SR.CategoryKeyImageSeries,
															  SR.NameKeyImageDocumentTitle,
															  SR.DescriptionKeyImageDocumentTitle, codeValue));

										properties.Add(
											new ImageProperty(SR.CategoryKeyImageSeries,
															  SR.NameKeyImageDocumentDescription,
															  SR.DescriptionKeyImageDocumentDescription, documentDescription));
									}
								}
							}
						}
					}
				}
			}

			return properties.ToArray();
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[ExtensionOf(typeof(ImagePropertyProviderExtensionPoint))]
	public class KeyObjectImagePropertyProvider : IImagePropertyProvider
	{
		public KeyObjectImagePropertyProvider()
		{
		}

		#region IImagePropertyProvider Members

		public IImageProperty[] GetProperties(IPresentationImage image)
		{
			List<IImageProperty> properties = new List<IImageProperty>();

			if (image != null && image.ParentDisplaySet != null)
			{
				IImageViewer viewer = image.ImageViewer;
				if (viewer != null)
				{
					string uid = image.ParentDisplaySet.Uid;
					if (!String.IsNullOrEmpty(uid))
					{
						StudyTree studyTree = viewer.StudyTree;
						Series keyObjectSeries = studyTree.GetSeries(uid);
						if (keyObjectSeries != null && keyObjectSeries.Sops.Count > 0)
						{
							Sop keyObjectSop = keyObjectSeries.Sops[0];
							if (keyObjectSop.SopClassUID == SopClass.KeyObjectSelectionDocumentStorageUid)
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

									properties.Add(
										new ImageProperty(SR.CategoryKeyImageSeries,
														  SR.NameKeyImageDocumentTitle,
														  SR.DescriptionKeyImageDocumentTitle, codeValue));

									properties.Add(
										new ImageProperty(SR.CategoryKeyImageSeries,
														  SR.NameKeyImageDocumentDescription,
														  SR.DescriptionKeyImageDocumentDescription, content.TextValue));
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

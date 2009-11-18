#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
	[ExtensionOf(typeof (ImagePropertyProviderExtensionPoint))]
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
									GeneralEquipmentModuleIod equipment = iod.GeneralEquipment;

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
											new ImageProperty("KeyImageDocumentTitle",
															  SR.CategoryKeyImageSeries,
											                  SR.NameKeyImageDocumentTitle,
											                  SR.DescriptionKeyImageDocumentTitle,
											                  codeValue));

										properties.Add(
											new ImageProperty("KeyImageDocumentDescription",
															  SR.CategoryKeyImageSeries,
											                  SR.NameKeyImageDocumentDescription,
											                  SR.DescriptionKeyImageDocumentDescription,
											                  documentDescription));

										properties.Add(
											new ImageProperty("KeyImageEquipmentManufacturer",
															  SR.CategoryKeyImageEquipment,
											                  SR.NameManufacturer,
											                  SR.DescriptionManufacturer,
											                  equipment.Manufacturer ?? ""));
										properties.Add(
											new ImageProperty("KeyImageEquipmentManufacturersModelName", 
															  SR.CategoryKeyImageEquipment,
											                  SR.NameManufacturersModelName,
											                  SR.DescriptionManufacturersModelName,
											                  equipment.ManufacturersModelName ?? ""));
										properties.Add(
											new ImageProperty("KeyImageEquipmentSoftwareVersions",
															  SR.CategoryKeyImageEquipment,
											                  SR.NameSoftwareVersions,
											                  SR.DescriptionSoftwareVersions,
											                  equipment.SoftwareVersions ?? ""));
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
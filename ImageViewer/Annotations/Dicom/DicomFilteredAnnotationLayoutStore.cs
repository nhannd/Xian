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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	internal sealed class DicomFilteredAnnotationLayoutStore
	{
		private static readonly DicomFilteredAnnotationLayoutStore _instance = new DicomFilteredAnnotationLayoutStore();

		private readonly object _syncLock = new object();
		private XmlDocument _document;

		private DicomFilteredAnnotationLayoutStore()
		{
			DicomFilteredAnnotationLayoutStoreSettings.Default.PropertyChanged +=
				delegate
				{
					this.Initialize(true);
				};
		}

		public static DicomFilteredAnnotationLayoutStore Instance
		{
			get { return _instance; }
		}

		public IList<DicomFilteredAnnotationLayout> FilteredLayouts
		{
			get
			{
				List<DicomFilteredAnnotationLayout> allFilteredLayouts = new List<DicomFilteredAnnotationLayout>();

				string xPath = "dicom-filtered-annotation-layout-configuration/dicom-filtered-annotation-layouts/dicom-filtered-annotation-layout";
				
				lock (_syncLock)
				{
					XmlNodeList filteredLayoutNodes = _document.SelectNodes(xPath);
					foreach (XmlElement filteredLayoutNode in filteredLayoutNodes)
						allFilteredLayouts.Add(DeserializeFilteredLayout(filteredLayoutNode));
				}

				return allFilteredLayouts;
			}
		}

		private void Initialize(bool reloadSettings)
		{
			lock (_syncLock)
			{
				if (_document != null && !reloadSettings)
					return;

				try
				{
					_document = new XmlDocument();

					if (!String.IsNullOrEmpty(DicomFilteredAnnotationLayoutStoreSettings.Default.FilteredLayoutSettingsXml))
					{
						_document.LoadXml(DicomFilteredAnnotationLayoutStoreSettings.Default.FilteredLayoutSettingsXml);
					}
					else
					{
						XmlElement root = _document.CreateElement("dicom-filtered-annotation-layout-configuration");
						_document.AppendChild(root);
						root.AppendChild(_document.CreateElement("dicom-filtered-annotation-layouts"));

						SaveSettings(_document.OuterXml);
					}
				}
				catch
				{
					_document = null;
					throw;
				}
			}
		}

		private static void SaveSettings(string settingsXml)
		{
			DicomFilteredAnnotationLayoutStoreSettings.Default.FilteredLayoutSettingsXml = settingsXml;
			DicomFilteredAnnotationLayoutStoreSettings.Default.Save();
		}

		private static DicomFilteredAnnotationLayout DeserializeFilteredLayout(XmlElement dicomFilteredLayoutNode)
		{
			string matchingLayoutId = dicomFilteredLayoutNode.GetAttribute("matching-layout-id");
			string filteredLayoutId = dicomFilteredLayoutNode.GetAttribute("id");

			DicomFilteredAnnotationLayout filteredLayout = new DicomFilteredAnnotationLayout(filteredLayoutId, matchingLayoutId);

			foreach (XmlElement filterNode in dicomFilteredLayoutNode.SelectNodes("filters/filter"))
			{
				string key = filterNode.GetAttribute("key");
				if (String.IsNullOrEmpty(key))
					continue;

				string filterValue = filterNode.GetAttribute("value");
				if (String.IsNullOrEmpty(filterValue))
					continue;

				filteredLayout.Filters.Add(new KeyValuePair<string, string>(key, filterValue));
			}

			return filteredLayout;
		}

		private static void SerializeFilteredLayout(XmlDocument document, DicomFilteredAnnotationLayout dicomFilteredAnnotationLayout)
		{
			string xPath = "dicom-filtered-annotation-layout-configuration/dicom-filtered-annotation-layouts";
			XmlElement filteredLayoutsNode = (XmlElement)document.SelectSingleNode(xPath);
			if (filteredLayoutsNode == null)
				throw new InvalidDataException(String.Format(SR.ExceptionInvalidFilteredAnnotationLayoutXml, "'dicom-filtered-annotation-layouts' node does not exist"));

			XmlElement newFilteredLayoutNode = document.CreateElement("dicom-filtered-annotation-layout");
			newFilteredLayoutNode.SetAttribute("id", dicomFilteredAnnotationLayout.Identifier);
			newFilteredLayoutNode.SetAttribute("matching-layout-id", dicomFilteredAnnotationLayout.MatchingLayoutIdentifier);

			XmlElement filtersNode = document.CreateElement("filters");
			newFilteredLayoutNode.AppendChild(filtersNode);

			foreach (KeyValuePair<string, string> keyValuePair in dicomFilteredAnnotationLayout.Filters)
			{
				XmlElement newFilterNode = document.CreateElement("filter");
				newFilterNode.SetAttribute("key", keyValuePair.Key);
				newFilterNode.SetAttribute("value", keyValuePair.Value);
				filtersNode.AppendChild(newFilterNode);
			}

			xPath = String.Format("dicom-filtered-annotation-layout[@id='{0}']", dicomFilteredAnnotationLayout.Identifier);
			XmlElement existingNode = (XmlElement)filteredLayoutsNode.SelectSingleNode(xPath);
			if (existingNode != null)
				filteredLayoutsNode.ReplaceChild(newFilteredLayoutNode, existingNode);
			else
				filteredLayoutsNode.AppendChild(newFilteredLayoutNode);
		}

		public void Clear()
		{
			SaveSettings("");
			Initialize(true);
		}

		public DicomFilteredAnnotationLayout GetFilteredLayout(string filteredLayoutId)
		{
			lock (_syncLock)
			{
				string xPath = String.Format("dicom-filtered-annotation-layout-configuration/dicom-filtered-annotation-layouts/dicom-filtered-annotation-layout[@id='{0}']", filteredLayoutId);
				XmlElement filteredLayoutNode = (XmlElement)_document.SelectSingleNode(xPath);
				if (filteredLayoutNode == null)
					return null;

				return DeserializeFilteredLayout(filteredLayoutNode);
			}
		}
		
		public void RemoveFilteredLayout(string filteredLayoutId)
		{
			lock (_syncLock)
			{
				string xPath = "dicom-filtered-annotation-layout-configuration/dicom-filtered-annotation-layouts";
				XmlElement filteredLayoutsNode = (XmlElement)_document.SelectSingleNode(xPath);
				if (filteredLayoutsNode == null)
					throw new InvalidDataException(String.Format(SR.ExceptionInvalidFilteredAnnotationLayoutXml, "'dicom-filtered-annotation-layouts' node does not exist"));

				xPath = String.Format("dicom-filtered-annotation-layout[@id='{0}']", filteredLayoutId);
				XmlElement filteredLayoutNode = (XmlElement)filteredLayoutsNode.SelectSingleNode(xPath);
				if (filteredLayoutNode != null)
					filteredLayoutsNode.RemoveChild(filteredLayoutNode);
			}
		}

		public void Update(IEnumerable<DicomFilteredAnnotationLayout> filteredLayouts)
		{
			lock (_syncLock)
			{
				Initialize(false);

				try
				{
					foreach (DicomFilteredAnnotationLayout filteredLayout in filteredLayouts)
					{
						Platform.CheckForNullReference(filteredLayout, "filteredLayout");
						Platform.CheckForEmptyString(filteredLayout.MatchingLayoutIdentifier, "filteredLayout.MatchingLayoutIdentifier");

						SerializeFilteredLayout(_document, filteredLayout);
					}

					SaveSettings(_document.OuterXml);
				}
				catch
				{
					Initialize(true);
					throw;
				}
			}
		}

		public void Update(DicomFilteredAnnotationLayout filteredLayout)
		{ 
			Platform.CheckForNullReference(filteredLayout, "filteredLayout");
			Platform.CheckForEmptyString(filteredLayout.MatchingLayoutIdentifier, "filteredLayout.MatchingLayoutIdentifier");

			lock (_syncLock)
			{
				Initialize(false);

				try
				{
					SerializeFilteredLayout(_document, filteredLayout);
					SaveSettings(_document.OuterXml);
				}
				catch
				{
					Initialize(true);
					throw;
				}
			}
		}

		public string GetMatchingStoredLayoutId(IImageSopProvider dicomImage)
		{
			if (dicomImage == null)
				return null;

			List<KeyValuePair<string, string>> filterCandidates = new List<KeyValuePair<string, string>>();

			//!!this is hard-coded as the only filter 'candidate' for now, until the more general uses cases are better understood.
			filterCandidates.Add(new KeyValuePair<string, string>("Modality", dicomImage.ImageSop.Modality));

			lock (_syncLock)
			{
				Initialize(false);

				string xPath = "dicom-filtered-annotation-layout-configuration/dicom-filtered-annotation-layouts/dicom-filtered-annotation-layout";
				XmlNodeList filteredLayoutNodes = _document.SelectNodes(xPath);
				foreach (XmlElement filteredLayoutNode in filteredLayoutNodes)
				{
					DicomFilteredAnnotationLayout filteredAnnotationLayout = DeserializeFilteredLayout(filteredLayoutNode);
					if (filteredAnnotationLayout.IsMatch(filterCandidates))
					{
						return filteredAnnotationLayout.MatchingLayoutIdentifier;
					}
				}
			}

			return "";
		}
	}
}

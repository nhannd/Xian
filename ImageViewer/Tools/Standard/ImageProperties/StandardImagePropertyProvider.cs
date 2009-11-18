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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[ExtensionOf(typeof(ImagePropertyProviderExtensionPoint))]
	internal class StandardImagePropertyProvider : IImagePropertyProvider
	{
		private static readonly IResourceResolver _resolver = new ResourceResolver(typeof(StandardImagePropertyProvider), false);

		public StandardImagePropertyProvider()
		{
		}

		private static XmlDocument LoadDocumentFromResources()
		{
			XmlDocument document = new XmlDocument();
			Stream xmlStream = _resolver.OpenResource("StandardImageProperties.xml");
			document.Load(xmlStream);
			xmlStream.Close();
			return document;
		}

		#region IImageInformation Members

		public IImageProperty[] GetProperties(IPresentationImage image)
		{
			List<IImageProperty> properties = new List<IImageProperty>();

			if (image == null || !(image is IImageSopProvider))
				return properties.ToArray();

			ISopDataSource dataSource = ((IImageSopProvider) image).ImageSop.DataSource;

			try
			{
				XmlDocument document = ImagePropertiesSettings.Default.StandardImagePropertiesXml;
				if (document == null)
				{
					Platform.Log(LogLevel.Debug, "StandardImagePropertiesXml setting document is null.");
					document = LoadDocumentFromResources();
				}

				XmlNodeList groupNodes = document.SelectNodes("//standard-image-properties/image-property-group");
				if (groupNodes == null || groupNodes.Count == 0)
				{
					Platform.Log(LogLevel.Debug, "StandardImagePropertiesXml setting document is empty or incorrectly formatted.");

					document = LoadDocumentFromResources();
					groupNodes = document.SelectNodes("//standard-image-properties/image-property-group");
				}

				if (groupNodes == null || groupNodes.Count == 0)
				{
					Platform.Log(LogLevel.Debug, "StandardImagePropertiesXml setting document is empty or incorrectly formatted.");
					return properties.ToArray();
				}

				foreach (XmlElement groupNode in groupNodes)
				{
					string category = "";
					XmlAttribute categoryAttribute = groupNode.Attributes["name"];
					if (categoryAttribute != null)
						category = LookupCategory(categoryAttribute.Value);

					XmlNodeList propertyNodes = groupNode.SelectNodes("image-property");
					if (propertyNodes == null)
					{
						Platform.Log(LogLevel.Debug, "tag-variable name attribute is empty");
						continue;
					}

					foreach (XmlElement propertyNode in propertyNodes)
					{
						string tagVariableName = null;
						XmlAttribute tagVariableNameAttribute = propertyNode.Attributes["tag-variable-name"];
						if (tagVariableNameAttribute != null)
							tagVariableName = tagVariableNameAttribute.Value;
                        
						if (String.IsNullOrEmpty(tagVariableName))
						{
							Platform.Log(LogLevel.Debug, "tag-variable name attribute is empty");	
							continue;
						}

						DicomTag tag = DicomTagDictionary.GetDicomTag(tagVariableName);
						if (tag == null)
						{
							Platform.Log(LogLevel.Debug, "tag-variable name doesn't match a valid DicomTag");	
							continue;
						}

						//TODO (cr Oct 2009): add ability to look up by hex-value, too
						string tagName = LookupTagName(tagVariableName);
						if (String.IsNullOrEmpty(tagName))
							tagName = tag.Name;

						string description;
						XmlAttribute attribute = propertyNode.Attributes["description"];
						if (attribute != null)
							description = attribute.Value;
						else
							description = LookupTagDescription(tagVariableName);

						string separator = null;
						attribute = propertyNode.Attributes["separator"];
						if (attribute != null)
							separator = attribute.Value;

						try
						{
							ImageProperty property = ImageProperty.Create(dataSource[tag], category, tagName, description, separator);
							properties.Add(property);
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Debug, e, "Failed to create image property '{0}'.", tagName);
						}
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Failed to read in image properties xml.");
			}

			return properties.ToArray();
		}

		//TODO: rethink this - I don't really like that the values are then looked up in resources.  Too limiting for those adding to the xml doc.
		private static string LookupCategory(string category)
		{
			if (String.IsNullOrEmpty(category))
				return "";

			string lookup = String.Format("Category{0}", category);
			string resolved = _resolver.LocalizeString(lookup);
			if (lookup == resolved)
				return category;
			else
				return resolved;
		}

		private static string LookupTagName(string tagName)
		{
			if (String.IsNullOrEmpty(tagName))
				return "";

			string lookup = String.Format("Name{0}", tagName);
			string resolved = _resolver.LocalizeString(lookup);
			if (lookup == resolved)
				return tagName;
			else
				return resolved;
		}

		private static string LookupTagDescription(string tagName)
		{
			if (String.IsNullOrEmpty(tagName))
				return "";

			string lookup = String.Format("Description{0}", tagName);
			string resolved = _resolver.LocalizeString(lookup);
			if (lookup == resolved)
				return tagName;
			else
				return resolved;
		}
		#endregion
	}
}
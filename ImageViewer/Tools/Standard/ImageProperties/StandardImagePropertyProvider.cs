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
					XmlAttribute categoryAttribute = groupNode.Attributes["category"];
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
							ImageProperty property = ImageProperty.Create(tag, dataSource, category, tagName, description, separator);
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
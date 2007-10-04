using System;
using System.Xml;

namespace AppConfigCreator
{
	class Program
	{
		/// <summary>
		/// This is just a very simple tool that will combine app.config files from multiple projects together.
		/// It uses a 'layered' technique, replacing existing elements as it proceeeds through the list of source 
		/// config files.
		/// Note that it is not a robust tool, it only accounts for the following nodes explicitly:
		/// - configSections (applicationSettings and userSettings)
		/// - applicationSettings
		/// - userSettings
		/// - system.ServiceModel
		/// 
		/// all other elements are simply replaced as each source file is processed.
		/// </summary>
		static void Main(string[] args)
		{
			XmlDocument newDocument = new XmlDocument();

			string[] mainConfigurationElements = new string[]
				{
					"configSections",
					"applicationSettings",
					"userSettings",
					"system.serviceModel"
				};

			XmlElement newConfigurationElement = newDocument.CreateElement("configuration");
			newDocument.AppendChild(newConfigurationElement);

			//Append the direct child nodes of 'configuration' to enforce the order.
			foreach (string mainElement in mainConfigurationElements)
			{
				XmlElement newElement = newDocument.CreateElement(mainElement);
				newConfigurationElement.AppendChild(newElement);
			}

			for (int i = 0; i < args.Length; ++i)
			{
				string path = args[i];
				XmlDocument document = new XmlDocument();
				document.Load(path);

				XmlElement configurationElement = (XmlElement)document.SelectSingleNode("configuration");

				foreach (XmlElement element in configurationElement)
				{
					bool isMainElement = false;
					foreach (string mainElement in mainConfigurationElements)
					{
						if (element.Name == mainElement)
						{
							isMainElement = true;
							break;
						}
					}

					bool elementAlreadyExists = (newConfigurationElement.SelectSingleNode(element.Name) != null);

					//Anything that is not one of the main elements, simply replace it.
					if (!isMainElement || !elementAlreadyExists)
						UpdateNode(newConfigurationElement, element, element.Name);
				}

				string elementPath;
				string xPath;

				string[] sectionGroups = new string[]{"applicationSettings", "userSettings"};

				foreach (string sectionGroupName in sectionGroups)
				{
					elementPath = "configSections/sectionGroup";
					string attributeName = "name";

					XmlElement sectionGroup = configurationElement.SelectSingleNode(elementPath + String.Format("[@{0}='{1}']", attributeName, sectionGroupName)) as XmlElement;
					if (sectionGroup != null)
					{
						XmlElement newApplicationSettingsSectionGroup = CreateElement(newConfigurationElement, elementPath, attributeName, sectionGroupName);
						foreach (XmlElement section in sectionGroup)
						{
							xPath = string.Format("section[@name='{0}']", section.GetAttribute("name"));
							UpdateNode(newApplicationSettingsSectionGroup, section, xPath);
						}
					}
				}

				foreach (string sectionGroupName in sectionGroups)
				{
					elementPath = sectionGroupName;
					XmlElement configSection = configurationElement.SelectSingleNode(elementPath) as XmlElement;
					if (configSection != null)
					{
						foreach (XmlElement typeElement in configSection)
						{
							elementPath = string.Format("{0}/{1}", sectionGroupName, typeElement.Name);
							XmlElement newTypeElement = CreateElement(newConfigurationElement, elementPath, null, null);
							
							foreach (XmlElement settingElement in typeElement)
							{
								xPath = string.Format("setting[@name='{0}']", settingElement.GetAttribute("name"));
								UpdateNode(newTypeElement, settingElement, xPath);
							}
						}
					}
				}

				XmlElement serviceModelBindingsElement = configurationElement.SelectSingleNode("system.serviceModel/bindings") as XmlElement;
				if (serviceModelBindingsElement != null)
				{
					foreach (XmlElement bindingType in serviceModelBindingsElement)
					{
						elementPath = string.Format("system.serviceModel/bindings/{0}", bindingType.Name);
						XmlElement newServiceModelBindingTypeElement = CreateElement(newConfigurationElement, elementPath, null, null);

						foreach (XmlElement binding in bindingType)
						{
							xPath = string.Format("binding[@name='{0}']", binding.GetAttribute("name"));
							UpdateNode(newServiceModelBindingTypeElement, binding, xPath);
						}
					}
				}

				elementPath = "system.serviceModel/client";
				XmlElement serviceModelClientElement = configurationElement.SelectSingleNode(elementPath) as XmlElement;
				if (serviceModelClientElement != null)
				{
					XmlElement newServiceModelClientElement = CreateElement(newConfigurationElement, elementPath, null, null);

					foreach (XmlElement client in serviceModelClientElement)
					{
						xPath = string.Format("endpoint[@address='{0}']", client.GetAttribute("address"));
						UpdateNode(newServiceModelClientElement, client, xPath);
					}
				}
			}

			foreach (string mainElement in mainConfigurationElements)
			{
				XmlElement element = (XmlElement)newConfigurationElement.SelectSingleNode(mainElement);
				if (!element.HasChildNodes)
					newConfigurationElement.RemoveChild(element);
			}

			System.IO.File.Delete("app.config");
			newDocument.Save("app.config");
		}

		private static XmlElement CreateElement(XmlElement parentElement, string path, string attributeName, string attributeValue)
		{
			string filterPath = path;
			if (!String.IsNullOrEmpty(attributeName))
				filterPath += string.Format("[@{0}='{1}']", attributeName, attributeValue);

			XmlElement existingElement = parentElement.SelectSingleNode(filterPath) as XmlElement;
			if (existingElement != null)
				return existingElement;

			string[] nodeNames = path.Split(new char[] {'/'});

			XmlElement appendNode = parentElement;
			for (int i = 0; i < nodeNames.Length; ++i)
			{
				filterPath = nodeNames[i];
				if (i == nodeNames.Length - 1 && attributeName != null)
					filterPath += string.Format("[@{0}='{1}']", attributeName, attributeValue);

				XmlElement newNode = appendNode.SelectSingleNode(filterPath) as XmlElement;
				if (newNode == null)
				{
					newNode = appendNode.OwnerDocument.CreateElement(nodeNames[i]);
					appendNode.AppendChild(newNode);
				}

				appendNode = newNode;
			}

			if (!String.IsNullOrEmpty(attributeName))
				appendNode.SetAttribute(attributeName, attributeValue);

			return appendNode;
		}

		private static void UpdateNode(XmlNode parentElement, XmlNode updateChild, string path)
		{
			XmlElement existingElement = parentElement.SelectSingleNode(path) as XmlElement;
			if (existingElement != null)
				parentElement.ReplaceChild(parentElement.OwnerDocument.ImportNode(updateChild, true), existingElement);
			else
				parentElement.AppendChild(parentElement.OwnerDocument.ImportNode(updateChild, true));
		}
	}
}

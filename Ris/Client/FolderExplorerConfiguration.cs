using System;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public interface IFolderExplorerConfiguration
	{
		IEnumerable<IFolderSystem> ApplyUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems);
		IList<IFolder> ApplyUserFoldersCustomizations(string folderSystemId, IList<IFolder> folders);
		bool IsFolderSystemReadOnly(IFolderSystem folderSystem);
	}

	public class FolderExplorerConfiguration : IFolderExplorerConfiguration
	{
		#region Private Members

		private XmlDocument _xmlDoc;
		private readonly SettingProvider _settingXml;

		#endregion

		#region Constructor

		public delegate string SettingProvider();
		public FolderExplorerConfiguration(SettingProvider settingXml)
		{
			_settingXml = settingXml;
		}

		#endregion

		#region IFolderExplorerConfigurationDocument members

		public IEnumerable<IFolderSystem> ApplyUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems)
		{
			var orderedFolderSystems = new List<IFolderSystem>();
			var remainingFolderSystems = new List<IFolderSystem>(folderSystems);

			// order the items based on the order in the XML
			foreach (XmlElement folderSystemSpec in this.ListXmlFolderSystems())
			{
				var id = folderSystemSpec.GetAttribute("id");

				var item = CollectionUtils.SelectFirst(remainingFolderSystems, fs => Equals(fs.Id, id));
				if (item != null)
				{
					orderedFolderSystems.Add(item);
					remainingFolderSystems.Remove(item);
					if (remainingFolderSystems.Count == 0)
						break;
				}
			}

			orderedFolderSystems.AddRange(remainingFolderSystems);
			return orderedFolderSystems;
		}

		public IList<IFolder> ApplyUserFoldersCustomizations(string folderSystemId, IList<IFolder> folders)
		{
			var customizedFolders = new List<IFolder>();
			var remainingFolders = new List<IFolder>(folders);

			foreach (XmlElement customizationSpec in GetFolderCustomizationSpecsForFolderSystem(folderSystemId))
			{
				var folderFilter = GetFolderFilterFromCustomizatinSpec(customizationSpec);
				foreach (var matchingRemainingFolder in CollectionUtils.Select(remainingFolders, folderFilter))
				{
					CustomizeItem(customizationSpec, matchingRemainingFolder);

					customizedFolders.Add(matchingRemainingFolder);
					remainingFolders.Remove(matchingRemainingFolder);

					if (remainingFolders.Count == 0)
						break;
				}
			}

			customizedFolders.AddRange(remainingFolders);
			return customizedFolders;
		}

		public bool IsFolderSystemReadOnly(IFolderSystem folderSystem)
		{
			var xmlFolderSystem = FindXmlFolderSystem(folderSystem.Id);

			if (xmlFolderSystem == null)
				return false;

			var readonlySetting = xmlFolderSystem.GetAttribute("readonly");

			// can edit by default
			if (string.IsNullOrEmpty(readonlySetting))
				return false;

			return string.Compare(readonlySetting, "true", true) == 0;
		}

		#endregion

		#region FolderExplorerConfigurationDocument virtual methods

		protected virtual Predicate<IFolder> GetFolderFilterFromCustomizatinSpec(XmlElement element)
		{
			var cls = element.GetAttribute("class");
			return folder => folder.GetType().Name == cls;
		}

		protected virtual void CustomizeItem(XmlElement element, IFolder folder)
		{
			var visibleSetting = element.GetAttribute("visible");
			folder.Visible = string.IsNullOrEmpty(visibleSetting) || string.Compare(visibleSetting, "false", true) != 0;
		}

		protected virtual IList<XmlNode> GetFolderCustomizationSpecsForFolderSystem(string folderSystemId)
		{
			var xmlFolderSystem = FindXmlFolderSystem(folderSystemId);
			
			return xmlFolderSystem == null 
				? new List<XmlNode>() 
				: CollectionUtils.Select<XmlNode>(xmlFolderSystem.ChildNodes, n => n.Name == "folder-class");
		}

		#endregion

		#region Private Methods

		protected XmlDocument XmlDocument
		{
			get
			{
				if (_xmlDoc == null)
				{
					try
					{
						_xmlDoc = new XmlDocument();
						_xmlDoc.LoadXml(_settingXml());
					}
					catch (Exception)
					{
						// any exception loading the XML document should reset the reference to null
						// so that we don't try to work with an invalid document in the future
						_xmlDoc = null;
						throw;
					}
				}

				return _xmlDoc;
			}
			set { _xmlDoc = value; }
		}

		protected XmlElement GetFolderSystemsNode()
		{
			return (XmlElement)this.XmlDocument.GetElementsByTagName("folder-systems")[0];
		}

		/// <summary>
		/// Finds a stored folder system in the XML doc with the specified ID.
		/// </summary>
		/// <param name="id">The folder system ID</param>
		/// <returns>An "folder-system" element, or null if not found</returns>
		protected XmlElement FindXmlFolderSystem(string id)
		{
			return (XmlElement)this.GetFolderSystemsNode().SelectSingleNode(String.Format("/folder-systems/folder-system[@id='{0}']", id));
		}

		/// <summary>
		/// List the stored folder systems in the XML doc
		/// </summary>
		/// <returns>A list of "folder-system" element</returns>
		protected XmlNodeList ListXmlFolderSystems()
		{
			return this.GetFolderSystemsNode().SelectNodes(String.Format("/folder-systems/folder-system"));
		}

		/// <summary>
		/// Finds a folder with the specified id in the specified "folder-system" node.
		/// </summary>
		/// <param name="id">the id of the folder to find</param>
		/// <param name="xmlFolderSystem">the "folder-system" node to search in</param>
		/// <returns>the XmlElement of the folder if found, otherwise null</returns>
		protected XmlElement FindXmlFolder(string id, XmlElement xmlFolderSystem)
		{
			return (XmlElement)xmlFolderSystem.SelectSingleNode(String.Format("folder[@id='{0}']", id));
		}

		#endregion
	}
}
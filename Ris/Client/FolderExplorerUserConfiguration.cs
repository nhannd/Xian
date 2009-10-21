using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public interface IFolderExplorerUserConfigurationUpdater
	{
		void SaveUserFoldersCustomizations(IFolderSystem folderSystem, List<IFolder> customizedFolders);
		void SaveUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems);
	}

	public interface IFolderExplorerUserConfiguration : IFolderExplorerConfiguration, IFolderExplorerUserConfigurationUpdater
	{
		void CommitTransaction();
		void RollbackTransaction();
		void BeginTransaction();
		event EventHandler ChangesCommitted;
	}

	public class FolderExplorerUserConfiguration : FolderExplorerConfiguration, IFolderExplorerUserConfiguration
	{
		#region Private fields

		private event EventHandler _changesCommitted;
		private bool _transactionPending;
		private readonly Action<string> _updateSetting;

		#endregion

		#region Constructor

		public FolderExplorerUserConfiguration(string settingXml, Action<string> updateSetting)
			: base(settingXml)
		{
			_updateSetting = updateSetting;
		}

		#endregion

		#region IFolderExplorerUserConfiguration members

		/// <summary>
		/// Begins a transaction.
		/// </summary>
		public void BeginTransaction()
		{
			_transactionPending = true;
		}

		/// <summary>
		/// Rollsback the transaction.
		/// </summary>
		public void RollbackTransaction()
		{
			if (!_transactionPending)
				throw new InvalidOperationException("Must call BeginTransaction() first.");

			this.XmlDocument = null;     // force xml doc to be reloaded from stored setting
			_transactionPending = false;
		}

		/// <summary>
		/// Commits the transaction.
		/// </summary>
		public void CommitTransaction()
		{
			if (!_transactionPending)
				throw new InvalidOperationException("Must call BeginTransaction() first.");

			// copy document to setting
			var sb = new StringBuilder();
			var writer = new XmlTextWriter(new StringWriter(sb)) { Formatting = System.Xml.Formatting.Indented };
			this.XmlDocument.Save(writer);

			_updateSetting(sb.ToString());

			_transactionPending = false;

			// notify subscribers
			EventsHelper.Fire(_changesCommitted, this, EventArgs.Empty);
		}

		/// <summary>
		/// Saves the order of the specified folder sytems.
		/// </summary>
		/// <param name="folderSystems"></param>
		public void SaveUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems)
		{
			if (!_transactionPending)
				throw new InvalidOperationException("Must call BeginTransaction() first.");

			var replacementFolderSystems = this.XmlDocument.CreateElement("folder-systems");

			foreach (var folderSystem in folderSystems)
			{
				var existingFolderSystem = FindXmlFolderSystem(folderSystem.Id) ?? CreateXmlFolderSystem(folderSystem.Id);
				replacementFolderSystems.AppendChild(existingFolderSystem.CloneNode(true));
			}

			GetFolderSystemsNode().InnerXml = replacementFolderSystems.InnerXml;
		}

		/// <summary>
		/// For the specified <see cref="IFolderSystem"/>, saves customizations of paths and visibility of <see cref="IFolder"/> items in the list.
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="customizedFolders"></param>
		public void SaveUserFoldersCustomizations(IFolderSystem folderSystem, List<IFolder> customizedFolders)
		{
			if (!_transactionPending)
				throw new InvalidOperationException("Must call BeginTransaction() first.");

			var xmlFolderSystem = FindXmlFolderSystem(folderSystem.Id);
			var replacementFolderSystem = CreateXmlFolderSystem(folderSystem.Id);

			foreach (var folder in customizedFolders)
			{
				var newXmlFolder = CreateXmlFolder(folder);
				replacementFolderSystem.AppendChild(newXmlFolder);
			}

			var folderSystemsNode = GetFolderSystemsNode();
			if (xmlFolderSystem != null)
				xmlFolderSystem.InnerXml = replacementFolderSystem.InnerXml;
			else
				folderSystemsNode.AppendChild(replacementFolderSystem);
		}

		/// <summary>
		/// Indicates that the current user's folder/folder system customizations have been committed.
		/// </summary>
		public event EventHandler ChangesCommitted
		{
			add { _changesCommitted += value; }
			remove { _changesCommitted -= value; }
		}

		#endregion

		#region FolderExplorerConfiguration overrides

		protected override Predicate<IFolder> GetFolderFilterFromCustomizatinSpec(XmlElement element)
		{
			var id = element.GetAttribute("id");
			return f => f.Id == id;
		}

		protected override void CustomizeItem(XmlElement element, IFolder folder)
		{
			base.CustomizeItem(element, folder);

			var pathSetting = element.GetAttribute("path");
			if (!string.IsNullOrEmpty(pathSetting))
			{
				folder.FolderPath = new Desktop.Path(pathSetting);
			}
		}

		protected override IList<XmlNode> GetFolderCustomizationSpecsForFolderSystem(string folderSystemId)
		{
			var xmlFolderSystem = FindXmlFolderSystem(folderSystemId) ?? CreateXmlFolderSystem(folderSystemId);
			return CollectionUtils.Select<XmlNode>(xmlFolderSystem.ChildNodes, n => n.Name == "folder");
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Creates the specified folder system, but *does not* immediately append it to the xmlDoc.
		/// </summary>
		/// <param name="id">the id of the "folder-system" to create</param>
		/// <returns>An "folder-system" element</returns>
		protected XmlElement CreateXmlFolderSystem(string id)
		{
			var xmlFolderSystem = this.XmlDocument.CreateElement("folder-system");
			xmlFolderSystem.SetAttribute("id", id);
			return xmlFolderSystem;
		}

		/// <summary>
		/// Creates a "folder" node for insertion into an "folder-system" node in the Xml store.
		/// </summary>
		/// <param name="folder">the folder whose relevant properties are to be used to create the node</param>
		/// <returns>a "folder" element</returns>
		protected XmlElement CreateXmlFolder(IFolder folder)
		{
			var xmlFolder = this.XmlDocument.CreateElement("folder");

			xmlFolder.SetAttribute("id", folder.Id);
			xmlFolder.SetAttribute("path", folder.FolderPath.LocalizedPath);
			// Visible is not editable by the user, so no need to save it?
			//xmlFolder.SetAttribute("visible", folder.Visible.ToString());

			return xmlFolder;
		}

		#endregion
	}
}
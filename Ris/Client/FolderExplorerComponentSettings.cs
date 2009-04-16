#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Configuration;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides services for storing the folder and folder system structure 
	/// to an XML document, and rebuilding that folder structure from the document.
	/// </summary>
	[SettingsGroupDescription("Configures folder systems.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class FolderExplorerComponentSettings
	{
		public delegate void InsertFolderDelegate(IFolder folder);

		private XmlDocument _xmlDoc;
		private event EventHandler _userFolderSystemCustomizationsChanged;

		private FolderExplorerComponentSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		#region Public API

		/// <summary>
		/// Orders the folder systems based on what is in the XML document,
		/// and puts any items without an XML entry into the remainder list.
		/// </summary>
		/// <param name="folderSystems">Input list of folder systems</param>
		/// <param name="ordered"></param>
		/// <param name="remainder"></param>
		public void ApplyUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems, out List<IFolderSystem> ordered, out List<IFolderSystem> remainder)
		{
			OrderFolderSystems(folderSystems, this.ListXmlFolderSystems(), out ordered, out remainder);
		}

		/// <summary>
		/// Customizes the folders in the specified folder system according to what is in the XML document,
		/// and puts any items without an XML entry into the remainder list.
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="ordered"></param>
		/// <param name="remainder"></param>
		public void ApplyUserFoldersCustomizations(IFolderSystem folderSystem, out List<IFolder> ordered, out List<IFolder> remainder)
		{
			XmlElement xmlFolderSystem = FindXmlFolderSystem(folderSystem.Id) ?? CreateXmlFolderSystem(folderSystem.Id);
			CustomizeFolders(folderSystem.Folders, xmlFolderSystem.GetElementsByTagName("folder"), out ordered, out remainder);
		}

		/// <summary>
		/// Saves the order of the specified folder sytems.
		/// </summary>
		/// <param name="folderSystems"></param>
		public void SaveUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems)
		{
			XmlElement replacementFolderSystems = this.GetXmlDocument().CreateElement("folder-systems");

			foreach (IFolderSystem folderSystem in folderSystems)
			{
				XmlElement existingFolderSystem = FindXmlFolderSystem(folderSystem.Id) ?? CreateXmlFolderSystem(folderSystem.Id);
				replacementFolderSystems.AppendChild(existingFolderSystem.CloneNode(true));
			}

			GetFolderSystemsNode().InnerXml = replacementFolderSystems.InnerXml;

			this.FolderPathXml = _xmlDoc.OuterXml;
			this.Save();
		}

		/// <summary>
		/// For the specified <see cref="IFolderSystem"/>, saves customizations of paths and visibility of <see cref="IFolder"/> items in the list.
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="customizedFolders"></param>
		public void SaveUserFoldersCustomizations(IFolderSystem folderSystem, List<IFolder> customizedFolders)
		{
			XmlElement xmlFolderSystem = FindXmlFolderSystem(folderSystem.Id);
			XmlElement replacementFolderSystem = CreateXmlFolderSystem(folderSystem.Id);

			foreach (IFolder folder in customizedFolders)
			{
				XmlElement newXmlFolder = CreateXmlFolder(folder);
				replacementFolderSystem.AppendChild(newXmlFolder);
			}

			XmlElement folderSystemsNode = GetFolderSystemsNode();
			if (xmlFolderSystem != null)
				xmlFolderSystem.InnerXml = replacementFolderSystem.InnerXml;
			else
				folderSystemsNode.AppendChild(replacementFolderSystem);

			this.FolderPathXml = _xmlDoc.OuterXml;
			this.Save();
		}

		/// <summary>
		/// Raises the <see cref="UserFolderSystemCustomizationsChanged"/> to indicate that all user 
		/// customizations have been committed to the settings.
		/// </summary>
		/// <remarks>
		/// Should be called following calls of <see cref="SaveUserFoldersCustomizations"/> and/or <see cref="SaveUserFolderSystemsOrder"/>
		/// </remarks>
		public void CompleteUserFolderSystemCustomizations()
		{
			EventsHelper.Fire(_userFolderSystemCustomizationsChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Indicates that the current user's folder/folder system customizations have changed.
		/// </summary>
		public event EventHandler UserFolderSystemCustomizationsChanged
		{
			add { _userFolderSystemCustomizationsChanged += value; }
			remove { _userFolderSystemCustomizationsChanged -= value; }
		}

		#endregion

		#region Private Utility Methods

		/// <summary>
		/// Orders the items in the input list.
		/// </summary>
		/// <remarks>
		/// Orders the input items according to the order specified by the <paramref name="ordering"/> XML list,
		/// and puts the result into the <paramref name="ordered"/> list.  Any items not found in the XML ordering list
		/// are put into the <paramref name="remainder"/> list.
		/// </remarks>
		/// <param name="input"></param>
		/// <param name="ordering"></param>
		/// <param name="ordered"></param>
		/// <param name="remainder"></param>
		private void OrderFolderSystems(IEnumerable<IFolderSystem> input, XmlNodeList ordering,
			out List<IFolderSystem> ordered, out List<IFolderSystem> remainder)
		{
			ordered = new List<IFolderSystem>();
			remainder = new List<IFolderSystem>(input);

			// order the items based on the order in the XML
			foreach (XmlElement element in ordering)
			{
				string id = element.GetAttribute("id");

				IFolderSystem item = CollectionUtils.SelectFirst(remainder,
					delegate(IFolderSystem fs) { return Equals(fs.Id, id); });

				if (item != null)
				{
					ordered.Add(item);
					remainder.Remove(item);
					if (remainder.Count == 0)
						break;
				}
			}
		}

		/// <summary>
		/// Applies user customizations to the path and visibility of items in the input list.
		/// </summary>
		/// <remarks>
		/// Orders the input items according to the order specified by the <paramref name="ordering"/> XML list,
		/// and puts the result into the <paramref name="ordered"/> list.  Any items not found in the XML ordering list
		/// are put into the <paramref name="remainder"/> list.
		/// </remarks>
		/// <param name="input"></param>
		/// <param name="ordering"></param>
		/// <param name="ordered"></param>
		/// <param name="remainder"></param>
		private void CustomizeFolders(IEnumerable<IFolder> input, XmlNodeList ordering, 
			out List<IFolder> ordered, out List<IFolder> remainder)
		{
			ordered = new List<IFolder>();
			remainder = new List<IFolder>(input);

			// order the items based on the order in the XML
			foreach (XmlElement element in ordering)
			{
				string id = element.GetAttribute("id");

				IFolder item = CollectionUtils.SelectFirst(remainder,
					delegate(IFolder x)
					{
						if (x is IWorklistFolder && !x.IsStatic)
							return Equals(((IWorklistFolder) x).WorklistRef.ToString(false), id);
						else
							return Equals(x.Id, id);
					});

				if (item != null)
				{
					string pathSetting = element.GetAttribute("path");
					if (!string.IsNullOrEmpty(pathSetting))
					{
						item.FolderPath = new Path(pathSetting);
					}

					string visibleSetting = element.GetAttribute("visible");
					item.Visible = string.IsNullOrEmpty(visibleSetting) || string.Compare(visibleSetting, "false", true) != 0;

					ordered.Add(item);
					remainder.Remove(item);

					if (remainder.Count == 0)
						break;
				}
			}
		}

		private XmlDocument GetXmlDocument()
		{
			if (_xmlDoc == null)
			{
				try
				{
					_xmlDoc = new XmlDocument();
					_xmlDoc.LoadXml(this.FolderPathXml);
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

		private XmlElement GetFolderSystemsNode()
		{
			return (XmlElement)this.GetXmlDocument().GetElementsByTagName("folder-systems")[0];
		}

		/// <summary>
		/// Creates the specified folder system, but *does not* immediately append it to the xmlDoc.
		/// </summary>
		/// <param name="id">the id of the "folder-system" to create</param>
		/// <returns>An "folder-system" element</returns>
		private XmlElement CreateXmlFolderSystem(string id)
		{
			XmlElement xmlFolderSystem = this.GetXmlDocument().CreateElement("folder-system");
			xmlFolderSystem.SetAttribute("id", id);
			return xmlFolderSystem;
		}

		/// <summary>
		/// Creates a "folder" node for insertion into an "folder-system" node in the Xml store.
		/// </summary>
		/// <param name="folder">the folder whose relevant properties are to be used to create the node</param>
		/// <returns>a "folder" element</returns>
		private XmlElement CreateXmlFolder(IFolder folder)
		{
			XmlElement xmlFolder = this.GetXmlDocument().CreateElement("folder");

			xmlFolder.SetAttribute("id", folder is IWorklistFolder && !folder.IsStatic
				? ((IWorklistFolder) folder).WorklistRef.ToString(false) 
				: folder.Id);
			xmlFolder.SetAttribute("path", folder.FolderPath.LocalizedPath);
			xmlFolder.SetAttribute("visible", folder.Visible.ToString());

			return xmlFolder;
		}

		/// <summary>
		/// Finds a stored folder system in the XML doc with the specified ID.
		/// </summary>
		/// <param name="id">The folder system ID</param>
		/// <returns>An "folder-system" element, or null if not found</returns>
		private XmlElement FindXmlFolderSystem(string id)
		{
			return (XmlElement)this.GetFolderSystemsNode().SelectSingleNode(String.Format("/folder-systems/folder-system[@id='{0}']", id));
		}

		/// <summary>
		/// List the stored folder systems in the XML doc
		/// </summary>
		/// <returns>A list of "folder-system" element</returns>
		private XmlNodeList ListXmlFolderSystems()
		{
			return this.GetFolderSystemsNode().SelectNodes(String.Format("/folder-systems/folder-system"));
		}

		/// <summary>
		/// Finds a folder with the specified id in the specified "folder-system" node.
		/// </summary>
		/// <param name="id">the id of the folder to find</param>
		/// <param name="xmlFolderSystem">the "folder-system" node to search in</param>
		/// <returns>the XmlElement of the folder if found, otherwise null</returns>
		private XmlElement FindXmlFolder(string id, XmlElement xmlFolderSystem)
		{
			return (XmlElement)xmlFolderSystem.SelectSingleNode(String.Format("folder[@id='{0}']", id));
		}

		#endregion
	}
}

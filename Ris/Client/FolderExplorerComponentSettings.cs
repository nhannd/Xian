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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using System.Diagnostics;
using ClearCanvas.Common.Utilities;

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
		public void OrderFolderSystems(IEnumerable<IFolderSystem> folderSystems, out List<IFolderSystem> ordered, out List<IFolderSystem> remainder)
        {
			Order(folderSystems, this.ListXmlFolderSystems(), delegate(IFolderSystem fs) { return fs.Id; },
				out ordered, out remainder);
        }

		/// <summary>
		/// Orders the folders in the specified folder system according to what is in the XML document,
		/// and puts any items without an XML entry into the remainder list.
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="ordered"></param>
		/// <param name="remainder"></param>
		public void OrderFolders(IFolderSystem folderSystem, out List<IFolder> ordered, out List<IFolder> remainder)
		{
			XmlElement xmlFolderSystem = FindXmlFolderSystem(folderSystem.Id) ?? CreateXmlFolderSystem(folderSystem.Id);
			Order(folderSystem.Folders, xmlFolderSystem.GetElementsByTagName("folder"), delegate (IFolder f) { return f.Id; },
				out ordered, out remainder);
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
		/// <typeparam name="T"></typeparam>
		/// <param name="input"></param>
		/// <param name="ordering"></param>
		/// <param name="idProvider"></param>
		/// <param name="ordered"></param>
		/// <param name="remainder"></param>
		private void Order<T>(IEnumerable<T> input, XmlNodeList ordering, Converter<T, string> idProvider,
			out List<T> ordered, out List<T> remainder)
			where T : class
		{
			ordered = new List<T>();
			remainder = new List<T>(input);

			// order the items based on the order in the XML
			foreach (XmlElement element in ordering)
			{
				string id = element.GetAttribute("id");

				T item = CollectionUtils.SelectFirst(remainder,
					delegate(T x) { return Equals(idProvider(x), id); });

				if (item != null)
				{
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
                catch(Exception)
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

            xmlFolder.SetAttribute("id", folder.Id);
            xmlFolder.SetAttribute("path", folder.FolderPath.LocalizedPath);

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

		#region Not currently in use - may need these in future if we save the folder ordering per user

		/// <summary>
        /// Synchronizes persistent folders with the xml store.
        /// Refer to <see cref="BuildAndSynchronize"/> for more details.
        /// </summary>
        /// <param name="folderSystemID">the ID of the folder system</param>
        /// <param name="folderMap">the folders that are to be synchronized/added to the store</param>
        /// <returns>the "folder-system" node with the specified folderSystemID</returns>
        private XmlElement Synchronize(string folderSystemID, IDictionary<string, IFolder> folderMap)
        {
            bool changed = false;

            XmlElement xmlFolderSystem = FindXmlFolderSystem(folderSystemID);
            bool systemExists = (xmlFolderSystem != null);
            if (!systemExists)
                xmlFolderSystem = CreateXmlFolderSystem(folderSystemID);

            //make sure every folder has a pre-determined spot in the store, inserting
            //folders appropriately based on their path.  The algorithm guarantees 
            //that each folder will get put somewhere in the store.  Only static folders
            //are added to the xml store; otherwise, the non-static folders would 
            //be determining the positions of persistent folders in the store,
            //which is clearly the reverse of what should happen.
            foreach (IFolder folder in folderMap.Values)
            {
                if (folder.IsStatic)
                {
                    if (AppendFolderToXmlFolderSystem(xmlFolderSystem, folder))
                        changed = true;
                }
            }

            if (changed)
            {
                if (!systemExists)
                    this.GetFolderSystemsNode().AppendChild(xmlFolderSystem);

				//this.FolderPathXml = _xmlDoc.InnerXml;
				//this.Save();
            }

            XmlElement xmlFolderSystemClone = (XmlElement)xmlFolderSystem.CloneNode(true);

            foreach (IFolder folder in folderMap.Values)
            {
                if (!folder.IsStatic)
                    AppendFolderToXmlFolderSystem(xmlFolderSystemClone, folder);
            }

            return xmlFolderSystemClone;
        }

        /// <summary>
        /// Appends the specified folder to the specified XML system.  The path
        /// attribute of the folder to be inserted is compared with the path of the
        /// folders in the xml system and an appropriate place to insert the folder is determined
        /// based on the length of the common path.
        /// </summary>
        /// <param name="xmlFolderSystem">the "folder-system" node to insert an folder into</param>
        /// <param name="folder">the folder to be inserted</param>
        /// <returns>a boolean indicating whether anything was added/removed/modified</returns>
        private bool AppendFolderToXmlFolderSystem(XmlElement xmlFolderSystem, IFolder folder)
        {
            if (null != FindXmlFolder(folder.Id, xmlFolderSystem))
                return false;

            XmlNode insertionPoint = null;
            int highestScore = 0;
            string folderPath = folder.FolderPath.LocalizedPath;

            foreach (XmlElement xmlFolder in xmlFolderSystem.GetElementsByTagName("folder"))
            {
                string path = xmlFolder.GetAttribute("path");

                int currentScore = folderPath.Contains(path) || path.Contains(folderPath) ? Math.Min(folderPath.Length, path.Length) : 0;
                if (currentScore >= highestScore)
                {
                    insertionPoint = xmlFolder;
                    highestScore = currentScore;
                }
            }

            XmlElement newXmlFolder = CreateXmlFolder(folder);

            if (insertionPoint != null)
                xmlFolderSystem.InsertAfter(newXmlFolder, insertionPoint);
            else
                xmlFolderSystem.AppendChild(newXmlFolder);

            return true;
		}

		#endregion
	}
}

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
    [SettingsGroupDescription("Stores the folder structure settings")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class FolderExplorerComponentSettings
    {
        public delegate void InsertFolderDelegate(IFolder folder);

        private XmlDocument _xmlDoc;

        private FolderExplorerComponentSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }

        /// <summary>
        /// Builds an in-memory folder system from the specified XML system and the specified set of folders.
        /// The folders will be ordered according to the XML system.  Any folders that are not a part of the
        /// XML system will be added to the memory folder system and inserted into the XML system based on the path.
        /// The XML system is automatically persisted, and new systems that have never before been persisted
        /// will be added to the store.
        /// </summary>
        /// <param name="folderSystem">FolderSystem that contains lists of folders</param>
        /// <param name="insertFolderDelegate">A delegate to insert folder</param>
        public void BuildAndSynchronize(IFolderSystem folderSystem, InsertFolderDelegate insertFolderDelegate)
        {
            string folderSystemID = folderSystem.Id;

            IDictionary<string, IFolder> folderMap = BuildFolderMap(folderSystem.Folders);
            XmlElement xmlFolderSystem = Synchronize(folderSystemID, folderMap);

            Build(xmlFolderSystem, folderMap, insertFolderDelegate);
        }

        /// <summary>
        /// Ordered the folder systems based on what is in the XML.  Any new folder systems will be appended at the end
        /// </summary>
        /// <param name="folderSystems">a list of folder systems</param>
        /// <returns>A list of folder systems that is ordered based on the XML</returns>
        public List<IFolderSystem> OrderFolderSystems(List<IFolderSystem> folderSystems)
        {
            List<IFolderSystem> orderedList = new List<IFolderSystem>();

            // order the folderSystems based on the order in the XML
            foreach (XmlElement xmlFolderSystem in this.ListXmlFolderSystems())
            {
                string folderSystemID = xmlFolderSystem.GetAttribute("id");

                IFolderSystem folderSystem = CollectionUtils.SelectFirst(folderSystems, 
                    delegate(IFolderSystem fs) { return Equals(fs.Id, folderSystemID); });

                if (folderSystem != null)
                {
                    orderedList.Add(folderSystem);
                    folderSystems.Remove(folderSystem);
                    if (folderSystems.Count == 0)
                        break;
                }
            }

            // Add all remaining folder systems, which does not yet exist in the XML
            orderedList.AddRange(folderSystems);

            return orderedList;
        }

        #region Private Utility Methods

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
        /// Builds a map of folder IDs to folders.
        /// </summary>
        /// <param name="folders">the set of folders from which to build a map</param>
        /// <returns>a map of folder IDs to folders</returns>
        private IDictionary<string, IFolder> BuildFolderMap(IEnumerable<IFolder> folders)
        {
            Dictionary<string, IFolder> folderMap = new Dictionary<string, IFolder>();

            foreach (IFolder folder in folders)
                folderMap[folder.Id] = folder;

            return folderMap;
        }

        /// <summary>
        /// Creates the specified folder system, but *does not* immediately append it to the xmlDoc.
        /// Since not all folders are persistent (e.g. some could be generated), we need to figure
        /// out how many folders (if any) belonging to the node will be persisted in the store
        /// before adding the folder to the store.
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

        /// <summary>
        /// Builds an in-memory folder system from the specified XML system and the specified set of folders.
        /// The folders will be ordered according to the XML system.
        /// </summary>
        /// <param name="xmlFolderSystem">an XML "folder-system" node</param>
        /// <param name="folders">the set of folders that the system should contain</param>
        /// <param name="insertFolderDelegate">A delegate to insert folder</param>
        private void Build(XmlElement xmlFolderSystem, IDictionary<string, IFolder> folders, InsertFolderDelegate insertFolderDelegate)
        {
            // process xml system, inserting folders in order
            foreach (XmlElement xmlFolder in xmlFolderSystem.GetElementsByTagName("folder"))
            {
                string folderID = xmlFolder.GetAttribute("id");
                if (folders.ContainsKey(folderID))
                {
                    IFolder folder = folders[folderID];
                    folders.Remove(folderID);

                    // update the folder path from the xml
                    string path = xmlFolder.GetAttribute("path");
                    folder.FolderPath = new Path(path, folder.ResourceResolver);

                    // insert the folder into the system
                    insertFolderDelegate(folder);
                }
            }

            if (folders.Count > 0)
                Debug.Assert(false);
        }

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

                this.FolderPathXml = _xmlDoc.InnerXml;
                this.Save();
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
    }
}

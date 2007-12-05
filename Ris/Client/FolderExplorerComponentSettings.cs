using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using System.Diagnostics;

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
        /// <param name="folderSystems">FolderSystem that contains lists of folders</param>
        /// <param name="insertFolderDelegate">A delegate to insert folder</param>
        public void BuildAndSynchronize(IList<IFolderSystem> folderSystems, InsertFolderDelegate insertFolderDelegate)
        {
            Dictionary<string, IDictionary<string, IFolder>> xmlFolderSystemFolderMap = new Dictionary<string, IDictionary<string, IFolder>>();
            Dictionary<string, XmlElement> xmlFolderSystemMap = new Dictionary<string, XmlElement>();

            // Synchronize all folder systems
            foreach (IFolderSystem folderSystem in folderSystems)
            {
                string folderSystemID = folderSystem.Id;

                // remember the folder system map
                IDictionary<string, IFolder> folderMap = BuildFolderMap(folderSystem.Folders);
                xmlFolderSystemFolderMap.Add(folderSystemID, folderMap);

                // remember the modified xmlFolderSystem, since it may be different from the settings (which only store static folder)
                XmlElement xmlFolderSystem = Synchronize(folderSystemID, folderMap);
                xmlFolderSystemMap.Add(folderSystemID, xmlFolderSystem);
            }

            // Using the order in the setting, rebuild each folder systems 
            foreach (XmlElement xmlFolderSystem in this.ListXmlFolderSystems())
            {
                string folderSystemID = xmlFolderSystem.GetAttribute("id");

                if (xmlFolderSystemFolderMap.ContainsKey(folderSystemID))
                    Build(xmlFolderSystemMap[folderSystemID], xmlFolderSystemFolderMap[folderSystemID], insertFolderDelegate);
            }
        }

        public void BuildAndSynchronize(IFolderSystem folderSystem, InsertFolderDelegate insertFolderDelegate)
        {
            // Synchronize all folder systems
            string folderSystemID = folderSystem.Id;

            IDictionary<string, IFolder> folderMap = BuildFolderMap(folderSystem.Folders);
            XmlElement xmlFolderSystem = Synchronize(folderSystemID, folderMap);

            Build(xmlFolderSystem, folderMap, insertFolderDelegate);
        }

        #region Private Utility Methods

        private XmlDocument GetXmlDocument()
        {
            try
            {
                if (_xmlDoc == null)
                {
                    _xmlDoc = new XmlDocument();
                    _xmlDoc.LoadXml(this.FolderPathXml);
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e);
                this.Reset();

                _xmlDoc = new XmlDocument();
                _xmlDoc.Load(this.FolderPathXml);
            }

            return _xmlDoc;
        }

        private XmlElement GetFolderSystemsNode()
        {
            try
            {
                return (XmlElement)this.GetXmlDocument().GetElementsByTagName("folder-systems")[0];
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e);
                this.Reset();
                return (XmlElement)this.GetXmlDocument().GetElementsByTagName("folder-systems")[0];
            }
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

                int currentScore = path.Contains(folderPath) ? folderPath.Length : 0;
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

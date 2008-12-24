using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    /// <summary>
    /// Helper class to create a list of <see cref="BaseImageLevelUpdateCommand"/> 
    /// that can be applied to a Dicom image.
    /// </summary>
    public class ImageUpdateCommandBuilder
    {
        /// <summary>
        /// Builds a list of <see cref="BaseImageLevelUpdateCommand"/> for the specified study using the specified mapping template.
        /// </summary>
        /// <typeparam name="TMappingObject"></typeparam>
        /// <param name="storage"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method generates a list of <see cref="BaseImageLevelUpdateCommand"/> based on the mapping in <see cref="TMappingObject"/>.
        /// <see cref="TMappingObject"/> specifies which Dicom fields the application is interested in, using <see cref="DicomFieldAttribute"/>.
        /// For example, if the application needs to update the study instance uid and study date in an image with what's in the database, 
        /// it will define the mapping class as:
        /// <code>
        /// class StudyInfoMapping 
        /// {
        ///     [DicomField(DicomTags.StudyInstanceUid)]
        ///     public String StudyInstanceUid{
        ///         get{ ... }
        ///         set{ ... }
        ///     }
        /// 
        ///     [DicomField(DicomTags.StudyDate)]
        ///     public String StudyDate{
        ///         get{ ... }
        ///         set{ ... }
        ///     }
        /// }
        /// 
        /// ImageUpdateCommandBuilder builder = new ImageUpdateCommandBuilder();
        /// IList<BaseImageLevelUpdateCommand> commandList = builder.BuildCommands<StudyInfoMapping>(studystorage);
        /// 
        /// DicomFile file = new DicomFile("file.dcm");
        /// foreach(BaseImageUpdateCommand command in commandList)
        /// {
        ///     command.Apply(file);
        /// }
        /// 
        /// 
        /// </code>
        /// 
        /// </remarks>
        public IList<BaseImageLevelUpdateCommand> BuildCommands<TMappingObject>(StudyStorage storage)
        {
            IList<StudyStorageLocation> storageLocationList = StudyStorageLocation.FindStorageLocations(storage);
            Debug.Assert(storageLocationList != null && storageLocationList.Count > 0);
            StudyStorageLocation storageLocation = storageLocationList[0];
            return BuildCommands<TMappingObject>(storageLocation);
        }

        public IList<BaseImageLevelUpdateCommand> BuildCommands<TMappingObject>(StudyStorageLocation storageLocation)
        {
            StudyXml studyXml = GetStudyXml(storageLocation);

            List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();
            commandList.AddRange(BuildCommands(typeof(TMappingObject), studyXml));

            return commandList;
        }

        private static IList<BaseImageLevelUpdateCommand> BuildCommands(Type type, StudyXml studyXml)
        {
            List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();
            EntityDicomMap fieldMap = EntityDicomMapManager.Get(type);
            foreach (DicomTag tag in fieldMap.Keys)
            {
                ImageLevelUpdateEntry entry = new ImageLevelUpdateEntry();
                entry.TagPath.Tag = tag;
                entry.TagPath.Parents = null;
                entry.Value = FindAttributeValue(tag, studyXml);
                SetTagCommand cmd = new SetTagCommand();
                cmd.UpdateEntry = entry;
                commandList.Add(cmd);
            }
            return commandList;
        }

        private static StudyXml GetStudyXml(StudyStorageLocation storageLocation)
        {
            StudyXml studyXml = new StudyXml();
            string studyXmlPath = Path.Combine(storageLocation.GetStudyPath(), storageLocation.StudyInstanceUid + ".xml");
            using (Stream stream = new FileStream(studyXmlPath, FileMode.Open))
            {
                XmlDocument doc = new XmlDocument();
                StudyXmlIo.Read(doc, stream);
                studyXml.SetMemento(doc);
            }
            return studyXml;
        }

        private static String FindAttributeValue(DicomTag tag, StudyXml studyXml)
        {
            XmlDocument doc = studyXml.GetMemento(new StudyXmlOutputSettings());
            String xpath = String.Format("//Attribute[@Tag='{0}']", tag.HexString);
            //Platform.Log(LogLevel.Info, "Looking for {0}", xpath);
            XmlNode node = doc.SelectSingleNode(xpath);
            if (node == null)
                return null;
            else
                return node.InnerText;
        }

    }

    public class EntityDicomMap : Dictionary<DicomTag, PropertyInfo>
    {
    }

    public class EntityDicomMapManager : Dictionary<Type, EntityDicomMap>
    {
        private static EntityDicomMapManager _instance = new EntityDicomMapManager();
        public static EntityDicomMapManager Instance
        {
            get { return _instance; }
        }


        static EntityDicomMapManager()
        {
            AddEntity(typeof(Study));
            AddEntity(typeof(Patient));
            AddEntity(typeof(StudyStorage));
        }

        private static void AddEntity(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            EntityDicomMap entry = new EntityDicomMap();
            foreach (PropertyInfo prop in properties)
            {
                object[] attributes = prop.GetCustomAttributes(typeof(DicomFieldAttribute), true);
                foreach (DicomFieldAttribute attr in attributes)
                {
                    DicomTag tag = attr.Tag;
                    entry.Add(tag, prop);
                }
            }

            Instance.Add(type, entry);
        }

        public static EntityDicomMap Get(Type type)
        {
            if (Instance.ContainsKey(type))
                return Instance[type];
            else
            {
                AddEntity(type);
                return Instance[type];
            }
        }
    }
}
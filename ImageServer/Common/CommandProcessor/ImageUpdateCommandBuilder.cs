using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    public class UpdateTagCommand : IImageLevelUpdateCommand
    {
        private ImageLevelUpdateEntry _updateEntry;

        public UpdateTagCommand()
        {
            
        }

        #region IImageLevelCommand Members

        public void Apply(DicomFile file)
        {
            if (_updateEntry!=null)
            {
                DicomAttribute attr = FindAttribute(file.DataSet, UpdateEntry);
                if (attr != null)
                    attr.SetStringValue(UpdateEntry.GetStringValue());
            }
            
        }

        private DicomAttribute FindAttribute(DicomAttributeCollection collection, ImageLevelUpdateEntry entry)
        {
            if (collection == null)
                return null;

            if (entry.ParentTags != null)
            {
                foreach (DicomTag tag in entry.ParentTags)
                {
                    DicomAttribute sq = collection[tag] as DicomAttributeSQ;
                    if (sq == null)
                    {
                        throw new Exception(String.Format("Invalid tag value: {0}({1}) is not a SQ VR", tag, tag.Name));
                    }
                    if (sq.IsEmpty)
                    {
                        DicomSequenceItem item = new DicomSequenceItem();
                        sq.AddSequenceItem(item);
                    }

                    DicomSequenceItem[] items = sq.Values as DicomSequenceItem[];
                    Platform.CheckForNullReference(items, "items");
                    collection = items[0];
                }
            }

            return collection[entry.Tag];
        }


        public override string ToString()
        {
            return String.Format("File update: Set {0}={1}", UpdateEntry.Tag, UpdateEntry.Value);
        }
        #endregion

        #region IImageLevelCommand Members

        public ImageLevelUpdateEntry UpdateEntry
        {
            get { return _updateEntry; }
            set { _updateEntry = value; }
        }

        #endregion
    }

    public class UpdateTagCommandParser
    {
        public UpdateTagCommand Parse(XmlNode node)
        {
            UpdateTagCommand command = null;

            if (node.Name == "SetTag")
            {
                // TODO: Consider using plugin mechanism similar to server rules.

                string tagPath = node.Attributes["TagPath"].Value;
                string value = node.Attributes["Value"].Value;
                List<DicomTag> parents;
                DicomTag tag;
                ParseTagPath(tagPath, out parents, out tag);
                Debug.Assert(tag != null);

                ImageLevelUpdateEntry entry = new ImageLevelUpdateEntry();
                entry.ParentTags = parents;
                entry.Tag = tag;
                entry.Value = value;
                command = new UpdateTagCommand();
                command.UpdateEntry = entry;
            }

            return command;
        }

        private static void ParseTagPath(string tagPath, out List<DicomTag> parentTags, out DicomTag tag)
        {
            Platform.CheckForNullReference(tagPath, "tagPath");

            parentTags = null;
            tag = null;

            string[] tagPathComponents = tagPath.Split(',');
            if (tagPathComponents != null)
            {
                uint tagValue;
                if (tagPathComponents.Length > 1)
                {
                    parentTags = new List<DicomTag>();

                    for (int i = 0; i < tagPathComponents.Length - 1; i++)
                    {
                        tagValue = uint.Parse(tagPathComponents[i], NumberStyles.HexNumber);
                        DicomTag parent = DicomTagDictionary.GetDicomTag(tagValue);
                        if (parent == null)
                            throw new Exception(String.Format("Specified tag {0} is not in the dictionary", parent));
                        parentTags.Add(parent);
                    }
                }

                tagValue = uint.Parse(tagPathComponents[tagPathComponents.Length - 1], NumberStyles.HexNumber);
                tag = DicomTagDictionary.GetDicomTag(tagValue);
                if (tag == null)
                    throw new Exception(String.Format("Specified tag {0} is not in the dictionary", tag));

            }
        }
    }
    

    public class ImageUpdateCommandBuilder
    {

        public IList<IImageLevelUpdateCommand> BuildCommands<TObject>(StudyStorage storage)
        {
            IList<StudyStorageLocation> storageLocationList = StudyStorageLocation.FindStorageLocations(storage);
            Debug.Assert(storageLocationList != null && storageLocationList.Count > 0);
            StudyStorageLocation storageLocation = storageLocationList[0];
            return BuildCommands<TObject>(storageLocation);
        }

        public IList<IImageLevelUpdateCommand> BuildCommands<TObject>(StudyStorageLocation storageLocation)
        {
            StudyXml studyXml = GetStudyXml(storageLocation);

            List<IImageLevelUpdateCommand> commandList = new List<IImageLevelUpdateCommand>();
            commandList.AddRange(BuildCommands(typeof(TObject), studyXml));

            return commandList;
        }

        private static IList<IImageLevelUpdateCommand> BuildCommands(Type type, StudyXml studyXml)
        {
            List<IImageLevelUpdateCommand> commandList = new List<IImageLevelUpdateCommand>();
            EntityDicomMap fieldMap = EntityDicomMapManager.Get(type);
            foreach (DicomTag tag in fieldMap.Keys)
            {
                ImageLevelUpdateEntry entry = new ImageLevelUpdateEntry();
                entry.Tag = tag;
                entry.ParentTags = null;
                entry.Value = FindAttributeValue(tag, studyXml);
                UpdateTagCommand cmd = new UpdateTagCommand();
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

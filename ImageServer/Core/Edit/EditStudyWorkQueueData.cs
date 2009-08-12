using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Core.Edit
{
    public class UpdateItem
    {
        private DicomTag _dicomTag;
        private string _value;
        private string _originalValue;

        /// <summary>
        /// *** For serialization ***
        /// </summary>
        public UpdateItem()
        {
            
        }
        public UpdateItem(uint tag, string originalValue, string newValue)
        {
            DicomTag = DicomTagDictionary.GetDicomTag(tag);
            OriginalValue = originalValue;
            Value = newValue;
        }

        [XmlIgnore]
        public DicomTag DicomTag
        {
            get { return _dicomTag; }
            set { _dicomTag = value; }
        }

        [XmlAttribute("TagValue")]
        public string TagValue
        {
            get
            { 
                return _dicomTag.HexString; 
            }
            set { 
                // NO-OP 
                uint tag;
                if (uint.TryParse(value, NumberStyles.HexNumber, null, out tag))
                {
                    _dicomTag = DicomTagDictionary.GetDicomTag(tag);
                }
            }
        }

        [XmlAttribute("TagName")]
        public string TagName
        {
            get
            {
                return _dicomTag != null ? _dicomTag.Name : null;
            }
            set
            {
                // NO-OP 
            }
        }

        [XmlAttribute("Value")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [XmlAttribute("OriginalValue")]
        public string OriginalValue
        {
            get { return _originalValue; }
            set { _originalValue = value; }
        }
    }

    public class EditRequest
    {
        private List<UpdateItem> _updateEntries;
        private string _userId;
        private DateTime? _timeStamp;

        public List<UpdateItem> UpdateEntries
        {
            get { return _updateEntries; }
            set { _updateEntries = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public DateTime? TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }
    }

    public class EditStudyWorkQueueData
    {
        private EditRequest _request = new EditRequest();

        public EditRequest EditRequest
        {
            get { return _request; }
            set { _request = value; }
        }
    }

    public class EditStudyWorkQueueDataParser
    {
        public EditStudyWorkQueueData Parse(XmlElement element)
        {
            Platform.CheckForNullReference(element, "element");

            if (element.Name == "editstudy")
            {
                return ParseOldXml(element);
            }
            else
            {
                return XmlUtils.Deserialize<EditStudyWorkQueueData>(element);
            }
        }

        private EditStudyWorkQueueData ParseOldXml(XmlElement element)
        {
            EditStudyWorkQueueData data = new EditStudyWorkQueueData();

            WebEditStudyCommandCompiler compiler = new WebEditStudyCommandCompiler();
            List<BaseImageLevelUpdateCommand> updateCommands = compiler.Compile(element);

            foreach (BaseImageLevelUpdateCommand command in updateCommands)
            {
                if (data.EditRequest.UpdateEntries==null)
                    data.EditRequest.UpdateEntries = new List<UpdateItem>();

                // convert BaseImageLevelUpdateCommand to UpdateItem
                string value = command.UpdateEntry.Value != null ? command.UpdateEntry.Value.ToString() : null;
                data.EditRequest.UpdateEntries.Add(new UpdateItem(command.UpdateEntry.TagPath.Tag.TagValue, command.UpdateEntry.OriginalValue, value));
            }

            return data;
           
        }
    }
}
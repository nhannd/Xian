#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Core.Edit
{
    public class UpdateItem
    {
        private DicomTag _dicomTag;

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
    	public string Value { get; set; }

    	[XmlAttribute("OriginalValue")]
    	public string OriginalValue { get; set; }
    }

	/// <summary>
	/// Edit request descriptor
	/// </summary>
    public class EditRequest
    {
    	public List<UpdateItem> UpdateEntries { get; set; }

    	public string UserId { get; set; }

    	public string Reason { get; set; }

    	public DateTime? TimeStamp { get; set; }
    }

    public class EditStudyWorkQueueData
    {
    	public EditStudyWorkQueueData()
    	{
    		EditRequest = new EditRequest();
    	}

    	public EditRequest EditRequest { get; set; }
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
        	return XmlUtils.Deserialize<EditStudyWorkQueueData>(element);
        }

        private static EditStudyWorkQueueData ParseOldXml(XmlElement element)
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
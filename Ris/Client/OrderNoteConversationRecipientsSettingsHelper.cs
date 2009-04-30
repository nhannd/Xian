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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines the schema of the machine specific user settings in JSML.  This data-contract is never sent to the server.
    /// It is purely a local-contract for internal use.
    /// </summary>
    [DataContract]
    public class RecipientSettings
    {
        [DataMember]
        public string StaffId;

        [DataMember]
        public string StaffGroupName;

        [DataMember]
        public bool Checked;
    }

    [DataContract]
    public class OrderNoteConversationRecipientsSettings
    {
        public OrderNoteConversationRecipientsSettings()
        {
            this.Settings = new List<RecipientSettings>();
        }

        [DataMember]
        public List<RecipientSettings> Settings;
    }

    public class OrderNoteConversationRecipientsSettingsHelper
    {
        private readonly OrderNoteConversationRecipientsSettings _settings;

        private static OrderNoteConversationRecipientsSettingsHelper _instance;

        private OrderNoteConversationRecipientsSettingsHelper()
        {
            _settings = JsmlSerializer.Deserialize<OrderNoteConversationRecipientsSettings>(OrderNoteConversationComponentSettings.Default.DefaultRecipients);
            if (_settings == null)
                _settings = new OrderNoteConversationRecipientsSettings();
        }

        public static OrderNoteConversationRecipientsSettingsHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new OrderNoteConversationRecipientsSettingsHelper();

                return _instance;
            }
        }

        public List<RecipientSettings> DefaultRecipients
        {
            get { return _settings.Settings; }
			set { _settings.Settings = value; }
        }

        public List<string> StaffIDs
        {
            get
            {
                List<string> staffIds = new List<string>();
                CollectionUtils.ForEach(this.DefaultRecipients, 
                    delegate(RecipientSettings r)
                        {
                            if (!string.IsNullOrEmpty(r.StaffId))
                                staffIds.Add(r.StaffId);
                        });

                return staffIds;
            }
        }

        public List<string> StaffGroupNames
        {
            get
            {
                List<string> staffGroupNames = new List<string>();
                CollectionUtils.ForEach(this.DefaultRecipients,
                    delegate(RecipientSettings r)
                    {
                        if (!string.IsNullOrEmpty(r.StaffGroupName))
                            staffGroupNames.Add(r.StaffGroupName);
                    });

                return staffGroupNames;
            }
        }

        public bool GetCheckState(StaffSummary staff)
        {
            return CollectionUtils.Contains(this.DefaultRecipients,
                delegate(RecipientSettings r) { return Equals(r.StaffId, staff.StaffId) && r.Checked; });
        }

        public bool GetCheckState(StaffGroupSummary staffGroup)
        {
            return CollectionUtils.Contains(this.DefaultRecipients,
                delegate(RecipientSettings r) { return Equals(r.StaffGroupName, staffGroup.Name) && r.Checked; });
        }

        public void Save()
        {
            string serializedSetting = JsmlSerializer.Serialize(_settings, typeof(OrderNoteConversationRecipientsSettings).Name, false);
            OrderNoteConversationComponentSettings.Default.DefaultRecipients = serializedSetting;
            OrderNoteConversationComponentSettings.Default.Save();
        }
    }
}

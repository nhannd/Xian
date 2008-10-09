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

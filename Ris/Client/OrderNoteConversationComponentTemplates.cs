using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public partial class OrderNoteConversationComponent
	{
		enum RecipientType
		{
			Staff,
			Group
		}

		[DataContract]
		class TemplatesData
		{
			public TemplatesData()
			{
				this.Templates = new List<TemplateData>();
			}

			[DataMember]
			public List<TemplateData> Templates;
		}

		[DataContract]
		class TemplateData
		{
			public TemplateData()
			{
				this.Recipients = new List<RecipientData>();
			}

			/// <summary>
			/// The display name of the template, that is presented to the user.
			/// </summary>
			[DataMember]
			public string DisplayName;

			/// <summary>
			/// Name of staff group that should be selected as the "On behalf of" choice.
			/// </summary>
			[DataMember]
			public string OnBehalfOfGroup;

			/// <summary>
			/// Default set of recipients.
			/// </summary>
			[DataMember]
			public List<RecipientData> Recipients;

			/// <summary>
			/// Set of soft keys.
			/// </summary>
			[DataMember]
			public List<SoftKeyData> SoftKeys;

			public List<string> GetStaffRecipients()
			{
				return CollectionUtils.Map(
					CollectionUtils.Select(this.Recipients, r => r.Type == RecipientType.Staff),
					(RecipientData r) => r.Id);
			}

			public List<string> GetGroupRecipients()
			{
				return CollectionUtils.Map(
					CollectionUtils.Select(this.Recipients, r => r.Type == RecipientType.Group),
					(RecipientData r) => r.Id);
			}
		}

		[DataContract]
		class RecipientData
		{
			/// <summary>
			/// Type of recipient.
			/// </summary>
			[DataMember]
			public RecipientType Type;

			/// <summary>
			/// Staff ID or Group Name.
			/// </summary>
			[DataMember]
			public string Id;
		}

		[DataContract]
		class SoftKeyData
		{
			/// <summary>
			/// Text to display on button.
			/// </summary>
			[DataMember]
			public string ButtonName;

			/// <summary>
			/// Text to insert when button pressed.
			/// </summary>
			[DataMember]
			public string InsertText;
		}

		[DataContract]
		class SoftKeysData
		{
			/// <summary>
			/// Set of soft keys.
			/// </summary>
			[DataMember]
			public List<SoftKeyData> SoftKeys;
		}

		/// <summary>
		/// Parses templates XML document.
		/// </summary>
		private static List<TemplateData> LoadTemplates(string templatesXml)
		{
			if(string.IsNullOrEmpty(templatesXml))
				return new List<TemplateData>();

			var templatesData = JsmlSerializer.Deserialize<TemplatesData>(templatesXml);
			return templatesData.Templates;
		}

		/// <summary>
		/// Parses soft keys XML document.
		/// </summary>
		private static List<SoftKeyData> LoadSoftKeys(string softKeysXml)
		{
			if (string.IsNullOrEmpty(softKeysXml))
				return new List<SoftKeyData>();

			var data = JsmlSerializer.Deserialize<SoftKeysData>(softKeysXml);
			return data.SoftKeys;
		}
	}
}

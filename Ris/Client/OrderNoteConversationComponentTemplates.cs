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
			/// Note body.
			/// </summary>
			[DataMember]
			public string NoteContent;

			/// <summary>
			/// Specifies whether a note can be posted without any recipients.
			/// </summary>
			[DataMember]
			public bool AllowPostWithoutRecipients;

			/// <summary>
			/// Specifies whether other suggested recipients should be automatically added based on the conversation history.
			/// </summary>
			[DataMember]
			public bool SuggestOtherRecipients;

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

			/// <summary>
			/// Indicates whether the recipient is mandatory, in which case the user cannot remove it.
			/// </summary>
			[DataMember]
			public bool Mandatory;
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

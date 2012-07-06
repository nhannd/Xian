#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{

	[SettingsGroupDescription("Configures behaviour of the Order Notes folder system.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class OrderNoteboxFolderSystemSettings
	{
		[DataContract]
		internal class GroupFoldersData : DataContractBase
		{
			/// <summary>
			/// Deserialization constructor.
			/// </summary>
			public GroupFoldersData()
			{
				StaffGroupNames = new List<string>();
			}

			public GroupFoldersData(List<string> staffGroupNames)
			{
				StaffGroupNames = staffGroupNames;
			}

			/// <summary>
			/// List of staff groups for which folders are visible.
			/// </summary>
			[DataMember]
			public List<string> StaffGroupNames;
		}


		private OrderNoteboxFolderSystemSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public GroupFoldersData GroupFolders
		{
			get
			{
				return string.IsNullOrEmpty(this.GroupFoldersXml)
						? new GroupFoldersData()
						: JsmlSerializer.Deserialize<GroupFoldersData>(this.GroupFoldersXml);
			}
			set
			{
				this.GroupFoldersXml = JsmlSerializer.Serialize(value, "GroupFoldersData");
			}
		}
	}
}

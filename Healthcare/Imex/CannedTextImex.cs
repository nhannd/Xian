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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("CannedText")]
	public class CannedTextImex : XmlEntityImex<CannedText, CannedTextImex.CannedTextData>
	{
		[DataContract]
		public class CannedTextData
		{
			[DataMember]
			public string Name;

			[DataMember]
			public string Category;

			[DataMember]
			public string StaffId;

			[DataMember]
			public string StaffGroupName;

			[DataMember]
			public string Text;
		}


		#region Overrides

		protected override IList<CannedText> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			CannedTextSearchCriteria where = new CannedTextSearchCriteria();
			where.Name.SortAsc(0);

			return context.GetBroker<ICannedTextBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override CannedTextData Export(CannedText entity, IReadContext context)
		{
			CannedTextData data = new CannedTextData();
			data.Name = entity.Name;
			data.Category = entity.Category;
			data.Text = entity.Text;
			data.StaffId = entity.Staff == null ? null : entity.Staff.Id;
			data.StaffGroupName = entity.StaffGroup == null ? null : entity.StaffGroup.Name;
			return data;
		}

		protected override void Import(CannedTextData data, IUpdateContext context)
		{
			CreateOrUpdateCannedText(data.Name, data.Category, data.StaffId, data.StaffGroupName, data.Text, context);
		}

		#endregion

		private static void CreateOrUpdateCannedText(
			string name, 
			string category, 
			string staffId, 
			string staffGroupName, 
			string text,
			IPersistenceContext context)
		{
			try
			{
				// At least one of these should be populated
				if (staffId == null && staffGroupName == null)
					throw new Exception("A canned text has a staff or a staff group.  They cannot both be empty");

				if (staffId != null && staffGroupName != null)
					throw new Exception("A canned text has a staff or a staff group.  They cannot both exist");

				CannedTextSearchCriteria criteria = new CannedTextSearchCriteria();

				// We must search all these criteria because the combination of them form a unique key
				criteria.Name.EqualTo(name);
				criteria.Category.EqualTo(category);
				if (!string.IsNullOrEmpty(staffId))
					criteria.Staff.Id.EqualTo(staffId);
				if (!string.IsNullOrEmpty(staffGroupName))
					criteria.StaffGroup.Name.EqualTo(staffGroupName);

				ICannedTextBroker broker = context.GetBroker<ICannedTextBroker>();
				CannedText cannedText = broker.FindOne(criteria);

				cannedText.Text = text;
			}
			catch (EntityNotFoundException)
			{
				Staff staff = FindStaff(staffId, context);
				StaffGroup staffGroup = FindStaffGroup(staffGroupName, context);

				if (!string.IsNullOrEmpty(staffId) && staff == null)
					throw new Exception("The requested staff does not exist.");

				if (!string.IsNullOrEmpty(staffGroupName) && staffGroup == null)
					throw new Exception("The requested staff group does not exist.");

				CannedText cannedText = new CannedText();
				cannedText.Name = name;
				cannedText.Category = category;
				cannedText.Staff = staff;
				cannedText.StaffGroup = staffGroup;
				cannedText.Text = text;
				context.Lock(cannedText, DirtyState.New);
			}
		}

		private static Staff FindStaff(string staffId, IPersistenceContext context)
		{
			try
			{
				if (string.IsNullOrEmpty(staffId))
					return null;

				StaffSearchCriteria criteria = new StaffSearchCriteria();
				criteria.Id.EqualTo(staffId);

				IStaffBroker broker = context.GetBroker<IStaffBroker>();
				return broker.FindOne(criteria);
			}
			catch (EntityNotFoundException)
			{
				return null;
			}
		}

		private static StaffGroup FindStaffGroup(string staffGroupName, IPersistenceContext context)
		{
			try
			{
				if (string.IsNullOrEmpty(staffGroupName))
					return null;

				StaffGroupSearchCriteria criteria = new StaffGroupSearchCriteria();
				criteria.Name.EqualTo(staffGroupName);

				IStaffGroupBroker broker = context.GetBroker<IStaffGroupBroker>();
				return broker.FindOne(criteria);
			}
			catch (EntityNotFoundException)
			{
				return null;
			}
		}
	}
}

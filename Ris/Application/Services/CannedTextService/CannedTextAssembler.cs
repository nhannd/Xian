using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Application.Services.CannedTextService
{
	public class CannedTextAssembler
	{
		#region Synchronize Helpers

		/// <summary>
		/// This sync helper is specific for the Canned Text staff group collection.  StaffGroup update is disabled
		/// </summary>
		class StaffGroupSynchronizeHelper : CollectionSynchronizeHelper<StaffGroup, StaffGroupSummary>
		{
			private readonly IPersistenceContext _context;

			public StaffGroupSynchronizeHelper(IPersistenceContext context)
				: base(false, true)
			{
				_context = context;
			}

			protected override bool CompareItems(StaffGroup domainItem, StaffGroupSummary sourceItem)
			{
				return Equals(domainItem.GetRef(), sourceItem.StaffGroupRef);
			}

			protected override void AddItem(StaffGroupSummary sourceItem, ICollection<StaffGroup> domainList)
			{
				// we don't actually create a new StaffGroup in the database, just add it to the collection
				StaffGroup group = _context.Load<StaffGroup>(sourceItem.StaffGroupRef, EntityLoadFlags.Proxy);
				domainList.Add(group);
			}

			protected override void UpdateItem(StaffGroup domainItem, StaffGroupSummary sourceItem, ICollection<StaffGroup> domainList)
			{
				throw new System.NotImplementedException();
			}

			protected override void RemoveItem(StaffGroup domainItem, ICollection<StaffGroup> domainList)
			{
				domainList.Remove(domainItem);
			}
		}

		/// <summary>
		/// This sync helper is specific for the Canned Text staff collection.  Staff update is disabled
		/// </summary>
		class StaffSynchronizeHelper : CollectionSynchronizeHelper<Staff, StaffSummary>
		{
			private readonly IPersistenceContext _context;

			public StaffSynchronizeHelper(IPersistenceContext context)
				: base(false, true)
			{
				_context = context;
			}

			protected override bool CompareItems(Staff domainItem, StaffSummary sourceItem)
			{
				return Equals(domainItem.GetRef(), sourceItem.StaffRef);
			}

			protected override void AddItem(StaffSummary sourceItem, ICollection<Staff> domainList)
			{
				// we don't actually create a new StaffGroup in the database, just add it to the collection
				Staff staff = _context.Load<Staff>(sourceItem.StaffRef, EntityLoadFlags.Proxy);
				domainList.Add(staff);
			}

			protected override void UpdateItem(Staff domainItem, StaffSummary sourceItem, ICollection<Staff> domainList)
			{
				throw new System.NotImplementedException();
			}

			protected override void RemoveItem(Staff domainItem, ICollection<Staff> domainList)
			{
				domainList.Remove(domainItem);
			}
		}

		#endregion

		public CannedTextSummary GetCannedTextSummary(CannedText cannedText, IPersistenceContext context)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextSummary(
				cannedText.GetRef(),
				cannedText.Name,
				cannedText.Path,
				cannedText.Text,
				cannedText.Staff == null ? null : staffAssembler.CreateStaffSummary(cannedText.Staff, context),
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup));
		}

		public CannedTextDetail GetCannedTextDetail(CannedText cannedText, IPersistenceContext context)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextDetail(
				cannedText.Name,
				cannedText.Path,
				cannedText.Text,
				cannedText.Staff == null ? null : staffAssembler.CreateStaffSummary(cannedText.Staff, context),
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup));
		}

		public CannedText CreateCannedText(CannedTextDetail detail, IPersistenceContext context)
		{
			CannedText newCannedText = new CannedText();
			UpdateCannedText(newCannedText, detail, context);
			return newCannedText;
		}

		public void UpdateCannedText(CannedText cannedText, CannedTextDetail detail, IPersistenceContext context)
		{
			cannedText.Name = detail.Name;
			cannedText.Path = detail.Path;
			cannedText.Text = detail.Text;
			cannedText.Staff = detail.Staff == null ? null : context.Load<Staff>(detail.Staff.StaffRef, EntityLoadFlags.Proxy);
			cannedText.StaffGroup = detail.StaffGroup == null ? null : context.Load<StaffGroup>(detail.StaffGroup.StaffGroupRef, EntityLoadFlags.Proxy);
		}
	}
}

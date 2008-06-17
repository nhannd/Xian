using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Application.Services.CannedTextService
{
	public class CannedTextAssembler
	{
		public CannedTextIdentifierDetail CreateCannedTextIdentifierDetail(CannedTextIdentifier id, IPersistenceContext context)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			if (id == null)
				return new CannedTextIdentifierDetail();

			return new CannedTextIdentifierDetail(
				id.Name,
				id.Category,
				id.Staff == null ? null : staffAssembler.CreateStaffSummary(id.Staff, context),
				id.StaffGroup == null ? null : groupAssembler.CreateSummary(id.StaffGroup));
		}

		public void UpdateCannedTextIdentifierDetail(CannedTextIdentifier id, CannedTextIdentifierDetail detail, IPersistenceContext context)
		{
			id.Name = detail.Name;
			id.Category = detail.Category;
			id.Staff = detail.Staff == null ? null : context.Load<Staff>(detail.Staff.StaffRef, EntityLoadFlags.Proxy);
			id.StaffGroup = detail.StaffGroup == null ? null : context.Load<StaffGroup>(detail.StaffGroup.StaffGroupRef, EntityLoadFlags.Proxy);
		}

		public CannedTextSummary GetCannedTextSummary(CannedText cannedText, IPersistenceContext context)
		{

			return new CannedTextSummary(
				cannedText.GetRef(),
				CreateCannedTextIdentifierDetail(cannedText.CannedTextId, context),
				cannedText.Text);
		}

		public CannedTextDetail GetCannedTextDetail(CannedText cannedText, IPersistenceContext context)
		{
			return new CannedTextDetail(
				CreateCannedTextIdentifierDetail(cannedText.CannedTextId, context),
				cannedText.Text);
		}

		public CannedText CreateCannedText(CannedTextDetail detail, IPersistenceContext context)
		{
			CannedText newCannedText = new CannedText();
			UpdateCannedText(newCannedText, detail, context);
			return newCannedText;
		}

		public void UpdateCannedText(CannedText cannedText, CannedTextDetail detail, IPersistenceContext context)
		{
			UpdateCannedTextIdentifierDetail(cannedText.CannedTextId, detail.Id, context);
			cannedText.Text = detail.Text;
		}
	}
}

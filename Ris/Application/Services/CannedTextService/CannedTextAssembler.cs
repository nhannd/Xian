using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Application.Services.CannedTextService
{
	public class CannedTextAssembler
	{
		public CannedTextSummary GetCannedTextSummary(CannedText cannedText, IPersistenceContext context)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextSummary(
				cannedText.GetRef(),
				cannedText.Name,
				cannedText.Category,
				cannedText.Staff == null ? null : staffAssembler.CreateStaffSummary(cannedText.Staff, context),
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup),
				cannedText.Text);
		}

		public CannedTextDetail GetCannedTextDetail(CannedText cannedText, IPersistenceContext context)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextDetail(
				cannedText.Name,
				cannedText.Category,
				cannedText.Staff == null ? null : staffAssembler.CreateStaffSummary(cannedText.Staff, context),
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup),
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
			cannedText.Name = detail.Name;
			cannedText.Category = detail.Category;
			cannedText.Staff = detail.Staff == null ? null : context.Load<Staff>(detail.Staff.StaffRef, EntityLoadFlags.Proxy);
			cannedText.StaffGroup = detail.StaffGroup == null ? null : context.Load<StaffGroup>(detail.StaffGroup.StaffGroupRef, EntityLoadFlags.Proxy);
			cannedText.Text = detail.Text;
		}
	}
}

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
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextDetail(
				cannedText.Name,
				cannedText.Category,
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup),
				cannedText.Text);
		}

		public CannedText CreateCannedText(CannedTextDetail detail, Staff owner, IPersistenceContext context)
		{
			CannedText newCannedText = new CannedText();
			UpdateCannedText(newCannedText, detail, owner, context);
			return newCannedText;
		}

		public void UpdateCannedText(CannedText cannedText, CannedTextDetail detail, Staff owner, IPersistenceContext context)
		{
			cannedText.Name = detail.Name;
			cannedText.Category = detail.Category;
			cannedText.Staff = detail.IsPersonal ? owner : null;
			cannedText.StaffGroup = detail.IsGroup ? context.Load<StaffGroup>(detail.StaffGroup.StaffGroupRef, EntityLoadFlags.Proxy) : null;
			cannedText.Text = detail.Text;
		}
	}
}

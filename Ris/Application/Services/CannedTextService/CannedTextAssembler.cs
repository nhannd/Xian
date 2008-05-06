using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
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
				cannedText.Path,
				cannedText.Text,
				CollectionUtils.Map<Staff, StaffSummary>(cannedText.StaffSubscribers, 
					delegate(Staff staff) { return staffAssembler.CreateStaffSummary(staff, context); }),
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(cannedText.GroupSubscribers,
					delegate(StaffGroup group) { return groupAssembler.CreateSummary(group); }));
		}

		public CannedText CreateCannedTextForStaff(CannedTextDetail detail, Staff staff, IPersistenceContext context)
		{
			CannedText newCannedText = new CannedText(detail.Name, detail.Path, detail.Text);
			newCannedText.StaffSubscribers.Add(staff);
			return newCannedText;
		}

		public void UpdateCannedText(CannedText cannedText, CannedTextDetail detail, IPersistenceContext context)
		{
			cannedText.Name = detail.Name;
			cannedText.Path = detail.Path;
			cannedText.Text = detail.Text;			
		}
	}
}

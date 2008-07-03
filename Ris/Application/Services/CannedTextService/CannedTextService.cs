using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Application.Services.CannedTextService
{
	[ServiceImplementsContract(typeof(ICannedTextService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class CannedTextService : ApplicationServiceBase, ICannedTextService
	{
		#region ICannedTextService Members

		[ReadOperation]
		public ListCannedTextResponse ListCannedText(ListCannedTextRequest request)
		{
			CannedTextAssembler assembler = new CannedTextAssembler();
			List<CannedTextSearchCriteria> criterias = new List<CannedTextSearchCriteria>();

			CannedTextSearchCriteria personalCannedTextCriteria = new CannedTextSearchCriteria();
			personalCannedTextCriteria.Staff.EqualTo(this.CurrentUserStaff);
			criterias.Add(personalCannedTextCriteria);

			if (this.CurrentUserStaff.Groups != null && this.CurrentUserStaff.Groups.Count > 0)
			{
				CannedTextSearchCriteria groupCannedTextCriteria = new CannedTextSearchCriteria();
				groupCannedTextCriteria.StaffGroup.In(this.CurrentUserStaff.Groups);
				criterias.Add(groupCannedTextCriteria);
			}

			IList<CannedText> results = PersistenceContext.GetBroker<ICannedTextBroker>().Find(criterias.ToArray(), request.Page);

			List<CannedTextSummary> staffCannedText = CollectionUtils.Map<CannedText, CannedTextSummary>(results,
				delegate(CannedText cannedText)
					{
						return assembler.GetCannedTextSummary(cannedText, this.PersistenceContext);
					});

			return new ListCannedTextResponse(staffCannedText);
		}

		[ReadOperation]
		public GetCannedTextEditFormDataResponse GetCannedTextEditFormData(GetCannedTextEditFormDataRequest request)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new GetCannedTextEditFormDataResponse(
				staffAssembler.CreateStaffSummary(this.CurrentUserStaff, this.PersistenceContext),
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(this.CurrentUserStaff.Groups,
					delegate(StaffGroup group)
						{
							return groupAssembler.CreateSummary(group);
						}));
		}

		[ReadOperation]
		public LoadCannedTextForEditResponse LoadCannedTextForEdit(LoadCannedTextForEditRequest request)
		{
			ICannedTextBroker broker = PersistenceContext.GetBroker<ICannedTextBroker>();
			CannedText cannedText;
			
			if (request.CannedTextRef != null)
			{
				cannedText = broker.Load(request.CannedTextRef);
			}
			else
			{
				CannedTextSearchCriteria criteria = new CannedTextSearchCriteria();

				if (!string.IsNullOrEmpty(request.Name))
					criteria.Name.EqualTo(request.Name);

				if (!string.IsNullOrEmpty(request.Category))
					criteria.Category.EqualTo(request.Category);

				if (!string.IsNullOrEmpty(request.StaffId))
					criteria.Staff.Id.EqualTo(request.StaffId);

				if (!string.IsNullOrEmpty(request.StaffGroupName))
					criteria.StaffGroup.Name.EqualTo(request.StaffGroupName);

				cannedText = broker.FindOne(criteria);
			}

			CannedTextAssembler assembler = new CannedTextAssembler();
			return new LoadCannedTextForEditResponse(assembler.GetCannedTextDetail(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public AddCannedTextResponse AddCannedText(AddCannedTextRequest request)
		{
			try
			{
				if (string.IsNullOrEmpty(request.Detail.Name))
					throw new RequestValidationException(SR.ExceptionCannedTextNameRequired);

				if (string.IsNullOrEmpty(request.Detail.Category))
					throw new RequestValidationException(SR.ExceptionCannedTextCategoryRequired);

				CannedTextAssembler assembler = new CannedTextAssembler();
				CannedText cannedText = assembler.CreateCannedText(request.Detail, this.PersistenceContext);

				PersistenceContext.Lock(cannedText, DirtyState.New);
				PersistenceContext.SynchState();

				return new AddCannedTextResponse(assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));
			}
			catch (EntityValidationException)
			{
				string text = request.Detail.IsPersonal ? 
					string.Format("staff {0}, {1}", request.Detail.Staff.Name.FamilyName, request.Detail.Staff.Name.GivenName) :
					string.Format("{0} group", request.Detail.StaffGroup.Name);

				throw new RequestValidationException(string.Format(SR.ExceptionIdenticalCannedTextExist, text));
			}
		}

		[UpdateOperation]
		public UpdateCannedTextResponse UpdateCannedText(UpdateCannedTextRequest request)
		{
			CannedText cannedText = this.PersistenceContext.Load<CannedText>(request.CannedTextRef);

			CannedTextAssembler assembler = new CannedTextAssembler();
			assembler.UpdateCannedText(cannedText, request.Detail, this.PersistenceContext);

			PersistenceContext.SynchState();
			return new UpdateCannedTextResponse(assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public DeleteCannedTextResponse DeleteCannedText(DeleteCannedTextRequest request)
		{
			CannedText cannedText = this.PersistenceContext.Load<CannedText>(request.CannedTextRef, EntityLoadFlags.Proxy);
			PersistenceContext.GetBroker<ICannedTextBroker>().Delete(cannedText);

			return new DeleteCannedTextResponse();
		}

		#endregion
	}
}

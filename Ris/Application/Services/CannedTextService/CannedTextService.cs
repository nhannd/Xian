using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
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
			List<CannedTextSummary> staffCannedText = CollectionUtils.Map<CannedText, CannedTextSummary>(
				PersistenceContext.GetBroker<ICannedTextBroker>().FindCannedTextForStaff(this.CurrentUserStaff),
				delegate(CannedText cannedText)
					{
						return assembler.GetCannedTextSummary(cannedText, this.PersistenceContext);
					});

			return new ListCannedTextResponse(staffCannedText);
		}

		[UpdateOperation]
		public AddCannedTextResponse AddCannedText(AddCannedTextRequest request)
		{
			if (string.IsNullOrEmpty(request.Detail.Name))
			{
				throw new RequestValidationException(SR.ExceptionCannedTextNameRequired);
			}

			CannedTextAssembler assembler = new CannedTextAssembler();
			CannedText cannedText = assembler.CreateCannedTextForStaff(request.Detail, this.CurrentUserStaff, this.PersistenceContext);

			PersistenceContext.Lock(cannedText, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddCannedTextResponse(assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public UpdateCannedTextResponse UpdateCannedText(UpdateCannedTextRequest request)
		{
			CannedText cannedText = this.PersistenceContext.Load<CannedText>(request.CannedTextRef);

			CannedTextAssembler assembler = new CannedTextAssembler();
			assembler.UpdateCannedText(cannedText, request.Detail, this.PersistenceContext);

			return new UpdateCannedTextResponse(assembler.GetCannedTextSummary(cannedText, this.PersistenceContext));
		}

		[UpdateOperation]
		public DeleteCannedTextResponse DeleteCannedText(DeleteCannedTextRequest request)
		{
			CannedText worklist = this.PersistenceContext.Load<CannedText>(request.CannedTextRef, EntityLoadFlags.Proxy);
			PersistenceContext.GetBroker<ICannedTextBroker>().Delete(worklist);

			return new DeleteCannedTextResponse();
		}

		#endregion
	}
}

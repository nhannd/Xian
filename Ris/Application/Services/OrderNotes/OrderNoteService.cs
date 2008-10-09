using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.OrderNotes
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IOrderNoteService))]
    public class OrderNoteService : ApplicationServiceBase, IOrderNoteService
    {
        /// <summary>
        /// Implementation of <see cref="IWorklistQueryContext"/>.
        /// </summary>
        class NoteboxQueryContext : INoteboxQueryContext
        {
            private readonly ApplicationServiceBase _applicationService;
            private readonly SearchResultPage _page;
        	private readonly StaffGroup _staffGroup;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="service"></param>
            /// <param name="page"></param>
            /// <param name="staffGroup"></param>
            public NoteboxQueryContext(ApplicationServiceBase service, SearchResultPage page, StaffGroup staffGroup)
            {
                _applicationService = service;
                _page = page;
            	_staffGroup = staffGroup;
            }

            #region INoteboxQueryContext Members

            /// <summary>
            /// Gets the current user <see cref="Healthcare.Staff"/> object.
            /// </summary>
            public Staff Staff
            {
                get { return _applicationService.CurrentUserStaff; }
            }

			/// <summary>
			/// Gets the staff group (relevant only for group folders).
			/// </summary>
        	public StaffGroup StaffGroup
        	{
				get { return _staffGroup; }
        	}

        	/// <summary>
            /// Gets the working <see cref="Facility"/> associated with the current user, or null if no facility is associated.
            /// </summary>
            public Facility WorkingFacility
            {
                get { return _applicationService.WorkingFacility; }
            }

            /// <summary>
            /// Gets the <see cref="SearchResultPage"/> that specifies which page of the worklist is requested.
            /// </summary>
            public SearchResultPage Page
            {
                get { return _page; }
            }

            /// <summary>
            /// Obtains an instance of the specified broker.
            /// </summary>
            /// <typeparam name="TBrokerInterface"></typeparam>
            /// <returns></returns>
            public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
            {
                return _applicationService.PersistenceContext.GetBroker<TBrokerInterface>();
            }

            #endregion
        }
        
        #region IOrderNoteService Members

		[ReadOperation]
		public ListStaffGroupsResponse ListStaffGroups(ListStaffGroupsRequest request)
    	{
			Platform.CheckForNullReference(request, "request");

			StaffGroupAssembler assembler = new StaffGroupAssembler();
			return new ListStaffGroupsResponse(
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(this.CurrentUserStaff.Groups,
					delegate (StaffGroup g) { return assembler.CreateSummary(g);}));
		}

		[UpdateOperation]
		public AddStaffGroupsResponse AddStaffGroups(AddStaffGroupsRequest request)
    	{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.StaffGroups, "StaffGroups");

			CollectionUtils.ForEach(request.StaffGroups,
				delegate(StaffGroupSummary s)
				{
					StaffGroup group = PersistenceContext.Load<StaffGroup>(s.StaffGroupRef, EntityLoadFlags.Proxy);
					if(!group.Elective)
						throw new RequestValidationException(string.Format("You cannot be added to non-elective staff group '{0}'.", group.Name));
					group.AddMember(this.CurrentUserStaff);
				});

			return new AddStaffGroupsResponse();
		}

    	[ReadOperation]
        public QueryNoteboxResponse QueryNotebox(QueryNoteboxRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.NoteboxClass, "request.NoteboxClass");

            Notebox notebox = NoteboxFactory.Instance.CreateNotebox(request.NoteboxClass);
            SearchResultPage page = new SearchResultPage(0, new OrderNoteSettings().ItemsPerPage);
    		NoteboxQueryContext nqc = new NoteboxQueryContext(this, page, request.StaffGroupRef == null ?
				null : PersistenceContext.Load<StaffGroup>(request.StaffGroupRef));

            IList results = null;
            if (request.QueryItems)
            {
                // get the first page, up to the default max number of items per page
                results = notebox.GetItems(nqc);
            }

            int count = -1;
            if (request.QueryCount)
            {
                // if the items were already queried, and the number returned is less than the max per page,
                // then there is no need to do a separate count query
                if (results != null && results.Count < page.MaxRows)
                    count = results.Count;
                else
					count = notebox.GetItemCount(nqc);
            }

            if (request.QueryItems)
            {
                OrderNoteboxItemAssembler assembler = new OrderNoteboxItemAssembler();
                List<OrderNoteboxItemSummary> items = CollectionUtils.Map<OrderNoteboxItem, OrderNoteboxItemSummary>(
                    results,
                    delegate(OrderNoteboxItem item) { return assembler.CreateSummary(item, PersistenceContext); });

                return new QueryNoteboxResponse(items, count);
            }
            else
            {
                return new QueryNoteboxResponse(new List<OrderNoteboxItemSummary>(), count);
            }
        }

        [ReadOperation]
        public GetConversationResponse GetConversation(GetConversationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.OrderRef, "request.OrderRef");

            Order order = PersistenceContext.Load<Order>(request.OrderRef);

            // select only notes that are actually posted, and meet category filter
            List<OrderNote> notes = CollectionUtils.Select(order.Notes,
                delegate(OrderNote n)
                {
                    return n.IsPosted && (request.CategoryFilters == null || request.CategoryFilters.Count == 0
                           || request.CategoryFilters.Contains(n.Category));
                });

            OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
			if (request.CountOnly)
				return new GetConversationResponse(order.GetRef(), null, notes.Count);
			else 
				return new GetConversationResponse(
					order.GetRef(),
					CollectionUtils.Map<OrderNote, OrderNoteDetail>(notes,
						delegate (OrderNote n)
						{
							return noteAssembler.CreateOrderNoteDetail(n, CurrentUserStaff, PersistenceContext);
						}), 
					notes.Count);
        }

		[ReadOperation]
		public GetConversationEditorFormDataResponse GetConversationEditorFormData(
    		GetConversationEditorFormDataRequest request)
    	{
            StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();
			GetConversationEditorFormDataResponse response = new GetConversationEditorFormDataResponse(
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
					this.CurrentUserStaff.ActiveGroups,	// only active staff groups should be choices
					delegate (StaffGroup sg)
					{
						return groupAssembler.CreateSummary(sg);
					}));

            if (request.RecipientStaffIDs != null && request.RecipientStaffIDs.Count > 0)
            {
                StaffSearchCriteria criteria = new StaffSearchCriteria();
                criteria.Id.In(request.RecipientStaffIDs);
				response.RecipientStaffs = CollectionUtils.Map<Staff, StaffSummary, List<StaffSummary>>(
					PersistenceContext.GetBroker<IStaffBroker>().Find(criteria),
					delegate(Staff s)
					{
						return staffAssembler.CreateStaffSummary(s, PersistenceContext);
					});
            }

            if (request.RecipientStaffGroupNames != null && request.RecipientStaffGroupNames.Count > 0)
            {
                StaffGroupSearchCriteria criteria = new StaffGroupSearchCriteria();
                criteria.Name.In(request.RecipientStaffGroupNames);
                response.RecipientStaffGroups = CollectionUtils.Map<StaffGroup, StaffGroupSummary, List<StaffGroupSummary>>(
                    PersistenceContext.GetBroker<IStaffGroupBroker>().Find(criteria),
                    delegate(StaffGroup sg)
                    {
                        return groupAssembler.CreateSummary(sg);
                    });
            }

            return response;
    	}

    	[UpdateOperation]
        public AcknowledgeAndPostResponse AcknowledgeAndPost(AcknowledgeAndPostRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.OrderRef, "request.OrderRef");

            Order order = PersistenceContext.Load<Order>(request.OrderRef);

            // process acknowledgements first
            if (request.OrderNotesToAcknowledge != null)
            {
                List<OrderNote> notes = CollectionUtils.Map<EntityRef, OrderNote>(request.OrderNotesToAcknowledge,
                    delegate(EntityRef noteRef)
                    {
                        return PersistenceContext.Load<OrderNote>(noteRef);
                    });

                foreach (OrderNote note in notes)
                {
                    //validate that the note is actually associated with the correct order
                    if (!Equals(note.Order, order))
                        throw new ArgumentException("Attempt to acknowledge a note that is not associated with this order.");

                    note.Acknowledge(CurrentUserStaff);
                }
            }

            try
            {
                // process reply note
                OrderNote replyNote = null;
                OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
                if (request.OrderNote != null)
                {
                    replyNote = noteAssembler.CreateOrderNote(
                        request.OrderNote, order, CurrentUserStaff, true, PersistenceContext);
                }
                PersistenceContext.SynchState();

                if (replyNote != null)
					return new AcknowledgeAndPostResponse(noteAssembler.CreateOrderNoteDetail(replyNote, CurrentUserStaff, PersistenceContext));
                else
                    return new AcknowledgeAndPostResponse(null);
            }
            catch (NoteAcknowledgementException e)
            {
                // occurs when there are notes that this author must ack prior to posting a new note
                throw new RequestValidationException(e.Message);
            }
        }

        #endregion
    }
}

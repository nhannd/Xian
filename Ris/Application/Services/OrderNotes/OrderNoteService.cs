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

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="service"></param>
            /// <param name="page"></param>
            public NoteboxQueryContext(ApplicationServiceBase service, SearchResultPage page)
            {
                _applicationService = service;
                _page = page;
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
        public QueryNoteboxResponse QueryNotebox(QueryNoteboxRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.NoteboxClass, "request.NoteboxClass");

            Notebox notebox = NoteboxFactory.Instance.CreateNotebox(request.NoteboxClass);
            SearchResultPage page = new SearchResultPage(0, new OrderNoteSettings().ItemsPerPage);
            IList results = null;
            if (request.QueryItems)
            {
                // get the first page, up to the default max number of items per page
                results = notebox.GetItems(new NoteboxQueryContext(this, page));
            }

            int count = -1;
            if (request.QueryCount)
            {
                // if the items were already queried, and the number returned is less than the max per page,
                // then there is no need to do a separate count query
                if (results != null && results.Count < page.MaxRows)
                    count = results.Count;
                else
                    count = notebox.GetItemCount(new NoteboxQueryContext(this, null));
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
            return new GetConversationResponse(
                CollectionUtils.Map<OrderNote, OrderNoteDetail>(notes,
                    delegate (OrderNote n)
                    {
                        return noteAssembler.CreateOrderNoteDetail(n, PersistenceContext);
                    }));
        }

        [UpdateOperation]
        public AcknowledgeAndReplyResponse AcknowledgeAndReply(AcknowledgeAndReplyRequest request)
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
                    return new AcknowledgeAndReplyResponse(noteAssembler.CreateOrderNoteDetail(replyNote, PersistenceContext));
                else
                    return new AcknowledgeAndReplyResponse(null);
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

using System;
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
        #region IOrderNoteService Members

        [ReadOperation]
        public QueryNoteboxResponse QueryNotebox(QueryNoteboxRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.NoteboxClass, "request.NoteboxClass");

            MrnAssembler mrnAssembler = new MrnAssembler();
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            StaffAssembler staffAssembler = new StaffAssembler();
            List<OrderNoteboxItemSummary> items;
            if(request.NoteboxClass == "Inbox")
            {
                NoteReadActivitySearchCriteria where1 = new NoteReadActivitySearchCriteria();
                where1.Recipient.Staff.EqualTo(CurrentUserStaff);

                NoteReadActivitySearchCriteria where2 = new NoteReadActivitySearchCriteria();
                where1.Recipient.Group.In(CurrentUserStaff.Groups);

                IList<NoteReadActivity> activities = PersistenceContext.GetBroker<INoteReadActivityBroker>().Find(
                    new NoteReadActivitySearchCriteria[] {where1, where2});

                items = CollectionUtils.Map<NoteReadActivity, OrderNoteboxItemSummary>(activities,
                    delegate(NoteReadActivity a)
                    {
                        OrderNote note = (OrderNote)a.Note;
                        return new OrderNoteboxItemSummary(
                            note.GetRef(),
                            note.Order.GetRef(),
                            note.Order.Patient.GetRef(),
                            mrnAssembler.CreateMrnDetail(CollectionUtils.FirstElement(note.Order.Patient.Profiles).Mrn),
                            nameAssembler.CreatePersonNameDetail(CollectionUtils.FirstElement(note.Order.Patient.Profiles).Name),
                            CollectionUtils.FirstElement(note.Order.Patient.Profiles).DateOfBirth,
                            note.Order.AccessionNumber,
                            note.Order.DiagnosticService.Name,
                            note.Category,
                            note.PostTime,
                            staffAssembler.CreateStaffSummary(note.Author, PersistenceContext),
                            a.IsAcknowledged
                            );
                    });
            }
            else if(request.NoteboxClass == "SentItems")
            {
                OrderNoteSearchCriteria where = new OrderNoteSearchCriteria();
                where.Author.EqualTo(CurrentUserStaff);
                where.PostTime.IsNotNull();

                IList<OrderNote> orderNotes = PersistenceContext.GetBroker<IOrderNoteBroker>().Find(where);
                items = CollectionUtils.Map<OrderNote, OrderNoteboxItemSummary>(orderNotes,
                    delegate(OrderNote note)
                    {
                        return new OrderNoteboxItemSummary(
                            note.GetRef(),
                            note.Order.GetRef(),
                            note.Order.Patient.GetRef(),
                            mrnAssembler.CreateMrnDetail(CollectionUtils.FirstElement(note.Order.Patient.Profiles).Mrn),
                            nameAssembler.CreatePersonNameDetail(CollectionUtils.FirstElement(note.Order.Patient.Profiles).Name),
                            CollectionUtils.FirstElement(note.Order.Patient.Profiles).DateOfBirth,
                            note.Order.AccessionNumber,
                            note.Order.DiagnosticService.Name,
                            note.Category,
                            note.PostTime,
                            staffAssembler.CreateStaffSummary(note.Author, PersistenceContext),
                            note.IsFullyAcknowledged
                            );
                    });
            }
            else 
                throw new RequestValidationException(string.Format("{0} is not a valid notebox class name.", request.NoteboxClass));

            return new QueryNoteboxResponse(items, items.Count);
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
            if(request.OrderNotesToAcknowledge != null)
            {
                List<OrderNote> notes = CollectionUtils.Map<EntityRef, OrderNote>(request.OrderNotesToAcknowledge,
                    delegate(EntityRef noteRef)
                    {
                        return PersistenceContext.Load<OrderNote>(noteRef);
                    });

                foreach (OrderNote note in notes)
                {
                    //validate that the note is actually associated with the correct order
                    if(!order.Notes.Contains(note))
                        throw new Exception();  //TODO better exception

                    note.Acknowledge(CurrentUserStaff);
                }
            }

            //TODO: ensure all required ack's have been made

            // process reply note
            OrderNote replyNote = null;
            OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
            if(request.OrderNote != null)
            {
                replyNote = noteAssembler.CreateOrderNote(
                    request.OrderNote, order, CurrentUserStaff, true, PersistenceContext);
            }

            PersistenceContext.SynchState();

            if(replyNote != null)
                return new AcknowledgeAndReplyResponse(noteAssembler.CreateOrderNoteDetail(replyNote, PersistenceContext));
            else
                return new AcknowledgeAndReplyResponse(null);
        }

        #endregion
    }
}

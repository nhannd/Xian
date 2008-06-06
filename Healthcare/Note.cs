using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Note entity
    /// </summary>
	public partial class Note : ClearCanvas.Enterprise.Core.Entity
	{
        #region Constructors

        /// <summary>
        /// Constructor for creating a new note with recipients.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="author"></param>
        /// <param name="onBehalfOf"></param>
        /// <param name="body"></param>
        public Note(string category, Staff author, StaffGroup onBehalfOf, string body)
        {
            _category = category;
            _author = author;
        	_onBehalfOfGroup = onBehalfOf;
            _body = body;
            _postings = new HashedSet<NotePosting>();

            _creationTime = Platform.Time;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this note has been posted.
        /// </summary>
        public virtual bool IsPosted
        {
            get { return _postTime != null; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Posts the note with no recipients.
        /// </summary>
        public virtual void Post()
        {
            Post(Platform.Time, new Staff[]{}, new StaffGroup[]{});
        }

		/// <summary>
		/// Posts the note to the specified recipients.
		/// </summary>
		/// <param name="staffRecipients"></param>
		/// <param name="groupRecipients"></param>
		public virtual void Post(IEnumerable<Staff> staffRecipients, IEnumerable<StaffGroup> groupRecipients)
		{
			Post(Platform.Time, staffRecipients, groupRecipients);
		}

		/// <summary>
		/// Gets a value indicating whether this note can (should) be acknowledged by the specified staff.
		/// </summary>
		/// <param name="staff"></param>
		/// <returns></returns>
		public virtual bool CanAcknowledge(Staff staff)
		{
			return GetAcknowledgeablePostings(staff).Count > 0;
		}

        /// <summary>
        /// Marks this note as being acknowledged by the specified staff.
        /// </summary>
        /// <remarks>
        /// If the specified staff is not a recipient of the note,
        /// and is not a member of a <see cref="StaffGroup"/> that is a recipient,
        /// a <see cref="WorkflowException"/> will be thrown.
        /// </remarks>
        /// <param name="staff"></param>
        public virtual void Acknowledge(Staff staff)
        {
            // cannot acknowledge if not posted
            if(!IsPosted)
                throw new WorkflowException("Cannot acknowledge a note that has not been posted.");


            // find all un-acknowledged postings that this staff person could acknowledge
            List<NotePosting> acknowledgeablePostings = GetAcknowledgeablePostings(staff);

            // if none, this is a workflow exception
            if(acknowledgeablePostings.Count == 0)
                throw new NoteAcknowledgementException("The specified staff was either not a recipient of this note, or the note has already been acknowledged.");

            // acknowledge the posting
            foreach (NotePosting posting in acknowledgeablePostings)
            {
                posting.Acknowledge(staff);
            }

            // update the 'fully acknowledged' status of this note
            _isFullyAcknowledged = CollectionUtils.TrueForAll(_postings,
                delegate(NotePosting posting) { return posting.IsAcknowledged; });
        }


    	#endregion

        #region Overridables

        /// <summary>
        /// Called from <see cref="Post"/>, allowing subclasses to cancel the post operation by throwing an exception.
        /// </summary>
        /// <remarks>
        /// Override this method to perform validation prior to posting.  Throw an exception to cancel the post.
        /// </remarks>
        protected virtual void BeforePost()
        {

        }

        #endregion


        #region Helpers

        /// <summary>
        /// Posts the note with the specified post-time.
        /// </summary>
        /// <param name="postTime"></param>
        /// <param name="staffRecipients"></param>
        /// <param name="groupRecipients"></param>
        private void Post(DateTime postTime, IEnumerable<Staff> staffRecipients, IEnumerable<StaffGroup> groupRecipients)
        {
            // create postings for any recipients
			foreach (Staff recipient in staffRecipients)
            {
            	NotePosting posting = new StaffNotePosting(this, false, null, recipient);
                _postings.Add(posting);
            }
			foreach (StaffGroup recipient in groupRecipients)
			{
				NotePosting posting = new GroupNotePosting(this, false, null, recipient);
				_postings.Add(posting);
			}

            // give subclass a chance to do some processing
            BeforePost();

            // set the post time
            _postTime = postTime;
        }

		private List<NotePosting> GetAcknowledgeablePostings(Staff staff)
		{
			return CollectionUtils.Select(_postings, delegate(NotePosting posting) { return posting.CanAcknowledge(staff); });
		}
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
        }

        #endregion
    }
}

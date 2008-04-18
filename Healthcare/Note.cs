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
        /// Constructor for creating a new note.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="author"></param>
        /// <param name="body"></param>
        /// <param name="post"></param>
        public Note(string category, Staff author, string body, bool post)
            : this(category, author, body, new NoteRecipient[] { }, post)
        {
        }

        /// <summary>
        /// Constructor for creating a new note with recipients.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="author"></param>
        /// <param name="body"></param>
        /// <param name="recipients"></param>
        /// <param name="post"></param>
        public Note(string category, Staff author, string body, IEnumerable<NoteRecipient> recipients, bool post)
        {
            _category = category;
            _author = author;
            _body = body;
            _recipients = new List<NoteRecipient>(recipients);
            _readActivities = new HashedSet<NoteReadActivity>();

            _creationTime = Platform.Time;

            if (post)
                Post(_creationTime);
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

        /// <summary>
        /// Gets a value indicating whether this note has been posted and
        /// acknowledged by all parties that are expected to acknowledge it.
        /// </summary>
        public virtual bool IsFullyAcknowledged
        {
            get
            {
                return IsPosted && CollectionUtils.TrueForAll(_readActivities,
                    delegate(NoteReadActivity readActivity) { return readActivity.IsAcknowledged; });
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Posts the note.
        /// </summary>
        public virtual void Post()
        {
            Post(Platform.Time);
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


            // find all un-acknowledged reading that this staff person could acknowledge
            List<NoteReadActivity> acknowledgeableActivities = CollectionUtils.Select(_readActivities,
                delegate(NoteReadActivity a)
                {
                    return !a.IsAcknowledged && (a.Recipient.Staff.Equals(staff) || a.Recipient.Group.Members.Contains(staff));
                });

            // if none, this is a workflow exception
            if(acknowledgeableActivities.Count == 0)
                throw new WorkflowException("The specified staff was either not a recipient of this note, or the note has already been acknowledged.");

            // acknowledge the reading
            foreach (NoteReadActivity readActivity in acknowledgeableActivities)
            {
                readActivity.Acknowledge(staff);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Posts the note with the specified post-time.
        /// </summary>
        /// <param name="postTime"></param>
        private void Post(DateTime postTime)
        {
            // create read activities for any recipients
            foreach (NoteRecipient recipient in _recipients)
            {
                NoteReadActivity readActivity = new NoteReadActivity(this, recipient, false, null);
                _readActivities.Add(readActivity);
            }

            // set the post time
            _postTime = postTime;
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
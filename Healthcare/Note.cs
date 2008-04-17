using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System.Collections.Generic;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Note entity
    /// </summary>
	public partial class Note : ClearCanvas.Enterprise.Core.Entity
	{
        /// <summary>
        /// Constructor for creating a new note.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="author"></param>
        /// <param name="body"></param>
        /// <param name="post"></param>
        public Note(string category, Staff author, string body, bool post)
            : this(category, author, body, new NoteRecipient[]{}, post)
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

            _creationTime = Platform.Time;

            if(post)
                Post(_creationTime);
        }

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

        /// <summary>
        /// Posts the note.
        /// </summary>
        public virtual void Post()
        {
            Post(Platform.Time);
        }

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
	}
}
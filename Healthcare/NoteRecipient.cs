using System;
using System.Collections;
using System.Text;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// NoteRecipient component
    /// </summary>
	public partial class NoteRecipient
	{
        /// <summary>
        /// Creates a staff recipient.
        /// </summary>
        /// <param name="staff"></param>
        public NoteRecipient(Staff staff)
        {
            _staff = staff;
        }

        /// <summary>
        /// Creates a group recipient.
        /// </summary>
        /// <param name="group"></param>
        public NoteRecipient(StaffGroup group)
        {
            _group = group;
        }

        /// <summary>
        /// Gets a value indicating whether this instance represents a group recipient.
        /// </summary>
        public bool IsGroupRecipient
        {
            get { return _group != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance represents a staff recipient.
        /// </summary>
        public bool IsStaffRecipient
        {
            get { return _staff != null; }
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
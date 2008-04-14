using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// StaffGroup entity
    /// </summary>
	public partial class StaffGroup : ClearCanvas.Enterprise.Core.Entity
    {
        /// <summary>
        /// Add a member to this staff group.
        /// </summary>
        /// <param name="member"></param>
        public virtual void AddMember(Staff member)
        {
            _members.Add(member);
            member.Groups.Add(this);
        }

        /// <summary>
        /// Remove a member from this staff group.
        /// </summary>
        /// <param name="member"></param>
        public virtual void RemoveMember(Staff member)
        {
            _members.Remove(member);
            member.Groups.Remove(this);
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
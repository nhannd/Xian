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
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            StaffGroup that = obj as StaffGroup;
            if (that == null) return false;

            return Equals(this.Name, that.Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
	}
}
using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Staff entity
    /// </summary>
	public partial class Staff : Entity
	{
        private void CustomInitialize()
        {
        }

        #region Object overrides

        public override bool Equals(object that)
        {
            // TODO: implement a test for business-key equality
            return this.Name.Equals((that as Staff).Name);
        }

        public override int GetHashCode()
        {
            // TODO: implement a hash-code based on the business-key used in the Equals() method
            return this.Name.GetHashCode();
        }

        #endregion
    }
}
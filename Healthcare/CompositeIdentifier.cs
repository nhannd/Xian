using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// CompositeIdentifier component
    /// </summary>
	public partial class CompositeIdentifier
	{
        private void CustomInitialize()
        {
        }

        public string Format()
        {
            return string.Format("{0} {1}", _assigningAuthority, _id);
        }
	}
}
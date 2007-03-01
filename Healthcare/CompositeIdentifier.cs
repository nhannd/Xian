using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// CompositeIdentifier component
    /// </summary>
	public partial class CompositeIdentifier : IFormattable
	{
        private void CustomInitialize()
        {
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO interpret the format string according to custom-defined format characters
            return string.Format("{0} {1}", _assigningAuthority, _id);
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}
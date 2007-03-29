using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Note entity
    /// </summary>
    public partial class Note : IFormattable
	{
        private void CustomInitialize()
        {
        }

        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
        }

        /// <summary>
        /// Equivalence comparison which ignores validity range and time stamp
        /// </summary>
        /// <param name="that">The note to compare to</param>
        /// <returns>True if all fields other than the validity range and time stamp are the same, False otherwise</returns>
        public bool IsEquivalentTo(Note that)
        {
            return (that != null) &&
            ((this._comment == default(string)) ? (that._comment == default(string)) : this._comment.Equals(that._comment)) &&
            ((this._createdBy == default(ClearCanvas.Healthcare.Staff)) ? (that._createdBy == default(ClearCanvas.Healthcare.Staff)) : this._createdBy.Equals(that._createdBy)) &&
            ((this._category == default(ClearCanvas.Healthcare.NoteCategory)) ? (that._category == default(ClearCanvas.Healthcare.NoteCategory)) : this._category.Equals(that._category)) &&
            true;
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO interpret the format string according to custom-defined format characters
            StringBuilder sb = new StringBuilder();
            if (_category != null)
            {
                sb.Append(_category);
            }
            sb.AppendFormat("{0}, {1}", _comment, _createdBy);
            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public class TagName
    {
        #region NHibernate-specific members
        public TagName()
        {
        }

        protected virtual string InternalTagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }
        #endregion

        public TagName(string tagName)
        {
            // validate the input
            if (null == tagName)
				throw new System.ArgumentNullException("tagName", SR.ExceptionTagNameCannotBeNullOrEmpty);

            if (0 == tagName.Length)
				throw new System.ArgumentOutOfRangeException("tagName", SR.ExceptionTagNameCannotBeNullOrEmpty);

            _tagName = tagName;
        }

        public override string ToString()
        {
            return _tagName;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator string(TagName tagName)
        {
            return tagName.ToString();
        }

        private string _tagName;
    }
}

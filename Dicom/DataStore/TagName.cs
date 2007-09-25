using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public class TagName
    {
		private string _tagName;
		
		/// <summary>
        /// Constructor for NHibernate.
        /// </summary>
        private TagName()
        {
        }

        public TagName(string tagName)
        {
			SetTagName(tagName);
        }

		/// <summary>
		/// Property for NHibernate.
		/// </summary>
		protected virtual string InternalTagName
		{
			get { return _tagName; }
			set { SetTagName(value); }
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

		private void SetTagName(string tagName)
		{
			// validate the input
			if (String.IsNullOrEmpty(tagName))
				throw new System.ArgumentNullException("tagName", SR.ExceptionTagNameCannotBeNullOrEmpty);

			_tagName = tagName;
		}
	}
}

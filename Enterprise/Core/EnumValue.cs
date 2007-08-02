using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public class EnumValue : DomainObject
    {
        private string _code;
        private string _value;
        private string _description;

        protected EnumValue()
        {
        }

        /// <summary>
        /// </summary>
        public virtual string Code
        {
            get { return _code; }
            private set { _code = value; }
        }

        /// <summary>
        /// </summary>
        public virtual string Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        /// <summary>
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            private set { _description = value; }
        }

        /// <summary>
        /// Overridden to provide value-based hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _code.GetHashCode();
        }

        /// <summary>
        /// Overridden to provide value-based equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
                return true;
            return (obj.GetType() == this.GetType()) && (obj as EnumValue).Code == this.Code;
        }

    }
}

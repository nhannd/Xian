using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Core
{
    public class EnumValue : DomainObject
    {
        // these are the values we have been using in the Hibernate mapping files
        public const int CodeLength = 255;  // default SQL server varchar
        public const int ValueLength = 50;
        public const int DescriptionLength = 200;

        private string _code;
        private string _value;
        private string _description;

        protected EnumValue()
        {
        }

        /// <summary>
        /// This constructor is needed for unit tests, to create fake enum values.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        public EnumValue(string code, string value, string description)
        {
            _code = code;
            _value = value;
            _description = description;
        }

        /// <summary>
        /// </summary>
        [Required]
        [Unique]
        [Length(CodeLength)]
        public virtual string Code
        {
            get { return _code; }
            private set { _code = value; }
        }

        /// <summary>
        /// </summary>
        [Required]
        [Unique]
        [Length(ValueLength)]
        public virtual string Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        /// <summary>
        /// </summary>
        [Length(DescriptionLength)]
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

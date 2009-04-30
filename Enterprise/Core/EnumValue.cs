#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Core
{
	[DeactivationFlag("Deactivated")]
    public class EnumValue : DomainObject
    {
        // these are the values we have been using in the Hibernate mapping files
        public const int CodeLength = 255;  // default SQL server varchar
        public const int ValueLength = 50;
        public const int DescriptionLength = 200;

        private string _code;
        private string _value;
        private string _description;
        private float _displayOrder;
    	private bool _deactivated;

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
        /// Gets a value providing a relative position for sorting the enumeration.
        /// </summary>
        public virtual float DisplayOrder
        {
            get { return _displayOrder; }
            private set { _displayOrder = value; }
        }

		/// <summary>
		/// Gets a value indicating whether this enumerated value has been de-activated.
		/// </summary>
    	public virtual bool Deactivated
    	{
			get { return _deactivated; }
			private set { _deactivated = value; }
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
            EnumValue other = obj as EnumValue;
            if (other == null)
                return false;

            // must be of same class, in addition to having same code, since codes are not guaranteed
            // unique across classes
            return (other.GetClass() == this.GetClass()) && other.Code == this.Code;
        }

    }
}

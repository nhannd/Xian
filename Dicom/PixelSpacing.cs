#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom
{
	//TODO: move this?
	
	public class PixelSpacing : IEquatable<PixelSpacing>
    {
		
		#region Private Members
		
		double _row;
		double _column;

		#endregion
		
		/// <summary>
		/// Constructor for NHibernate.
		/// </summary>
		protected PixelSpacing()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="PixelSpacing"/>.
		/// </summary>
		public PixelSpacing(double row, double column)
		{
			_row = row;
			_column = column;
		}

		public bool IsNull
		{
			get { return _row == 0 || _column == 0; }
		}

		#region NHibernate Persistent Properties

		public virtual double Row
        {
            get { return _row; }
            protected set { _row = value; }
        }

        public virtual double Column
        {
            get { return _column; }
			protected set { _column = value; }
		}

		#endregion

		public override string ToString()
		{
			return String.Format(@"{0:F8}\{1:F8}", _row, _column);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is PixelSpacing)
				return this.Equals((PixelSpacing) obj);

			return false;
		}

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

		#region IEquatable<PixelSpacing> Members

		public bool Equals(PixelSpacing other)
		{
			if (other == null)
				return false;

			return this.Row == other.Row && this.Column == other.Column;
		}

		#endregion
	}
}

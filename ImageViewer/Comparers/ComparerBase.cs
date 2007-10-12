#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Base class for comparers that are used for sorting of collections.
	/// </summary>
	public abstract class ComparerBase
	{
		private int _returnValue;

		/// <summary>
		/// Initializes a new instance of <see cref="ComparerBase"/>.
		/// </summary>
		protected ComparerBase()
		{
			Reverse = false;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ComparerBase"/>.
		/// </summary>
		/// <param name="reverse"></param>
		protected ComparerBase(bool reverse)
		{
			this.Reverse = reverse;
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the collection will be
		/// sorted in ascending or descending order.
		/// </summary>
		public bool Reverse
		{
			get
			{ 
				return ( this.ReturnValue == 1); 
			}
			set
			{
				if (!value)
					this.ReturnValue = -1;
				else
					this.ReturnValue = 1;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the collection will be
		/// sorted in ascending or descending order.
		/// </summary>
		/// <value>1 if <see cref="Reverse"/> is <b>true</b></value>
		/// <value>-1 if <see cref="Reverse"/> is <b>false</b></value>
		protected int ReturnValue
		{
			get { return _returnValue; }
			set { _returnValue = value; }
		}
	}
}

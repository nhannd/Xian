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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Used to represent a specific parameter to a stored procedure.
    /// </summary>
    /// <typeparam name="T">The type associated with the parameter.</typeparam>
    public class ProcedureParameter<T> : SearchCriteria
    {
        private T _value;
		private bool _output = false;

		/// <summary>
		/// Constructor for input parameters.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
        public ProcedureParameter(String key, T value)
            : base(key)
        {
            _value = value;
        }

		/// <summary>
		/// Contructor for output parameters.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="output"></param>
		public ProcedureParameter(String key)
			: base(key)
		{
			_output = true;
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ProcedureParameter(ProcedureParameter<T> other)
            : base(other)
        {
            _value = other._value;
            _output = other._output;
        }

        public override object Clone()
        {
            return new ProcedureParameter<T>(this);
        }

		public bool Output
		{
			get { return _output; }
		}

		public T Value
		{
			get { return _value; }
			set { _value = value; }
		}
    }
}

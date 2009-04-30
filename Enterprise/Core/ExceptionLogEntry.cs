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
using ClearCanvas.Common;
using System.Threading;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Records information about an exception.
    /// </summary>
    public class ExceptionLogEntry : LogEntry
    {
        private string _exceptionClass;
        private string _message;
    	private string _assemblyName;
    	private string _assemblyLocation;


        /// <summary>
        /// Private no-args constructor to support NHibernate
        /// </summary>
        protected ExceptionLogEntry()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="e"></param>
        /// <param name="details"></param>
        public ExceptionLogEntry(string operation, Exception e, string details)
            :base(operation, details)
        {
            _exceptionClass = e.GetType().FullName;
            _message = e.Message;
			if(e.TargetSite != null)
			{
				Assembly assembly = e.TargetSite.DeclaringType.Assembly;
				_assemblyName = assembly.FullName;
				_assemblyLocation = assembly.Location;
			}
        }

        /// <summary>
        /// Gets or sets the name of the exception class.
        /// </summary>
        public string ExceptionClass
        {
            get { return _exceptionClass; }
            set { _exceptionClass = value; }
        }

        /// <summary>
        /// Gets or sets the top-level message exposed by the exception.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

		/// <summary>
		/// Gets or sets the name of the assembly that threw the exception.
		/// </summary>
    	public string AssemblyName
    	{
			get { return _assemblyName; }
			set { _assemblyName = value; }
    	}

		/// <summary>
		/// Gets or sets the disk location of the assembly that threw the exception.
		/// </summary>
		public string AssemblyLocation
    	{
			get { return _assemblyLocation; }
			set { _assemblyLocation = value; }
    	}
    }
}

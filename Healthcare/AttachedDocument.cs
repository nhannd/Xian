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
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare {


	/// <summary>
	/// AttachedDocument entity
	/// </summary>
	public partial class AttachedDocument : ClearCanvas.Enterprise.Core.Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Marks this document as having been attached.
		/// </summary>
		public virtual void Attach()
		{
			return;
		}

		/// <summary>
		/// Marks this document as being detached.
		/// </summary>
		public virtual void Detach()
		{
			
		}

		/// <summary>
		/// Summary of derived-class specific details of the attached document
		/// </summary>
		public virtual IDictionary<string, string> DocumentHeaders
		{
			get { return null; }
		}

		public virtual string DocumentTypeName
		{
			get { return "Attached Document"; }
		}

		public static string BuildContentUrl(AttachedDocument document, string pathDelimiter)
		{
			var builder = new StringBuilder();
			builder.Append(document.CreationTime.Year.ToString());
			builder.Append(pathDelimiter);
			builder.Append(document.CreationTime.Month.ToString());
			builder.Append(pathDelimiter);
			builder.Append(document.CreationTime.Day.ToString());
			builder.Append(pathDelimiter);
			builder.AppendFormat("{0}.{1}", document.GetRef().ToString(false, false), document.FileExtension);
			return builder.ToString();
		}
	}
}
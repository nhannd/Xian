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
using System.Collections;
using System.IO;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using Iesi.Collections.Generic;
using System.Xml;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ProcedureType entity
    /// </summary>
	public partial class ProcedureType
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public ProcedureType(string id, string name)
            :this(id, name, null, null)
        {
        }

        /// <summary>
        /// Sets the plan for this procedure type from the specified prototype procedure.
        /// </summary>
        /// <param name="prototype"></param>
        public virtual void SetPlanFromPrototype(Procedure prototype)
        {
            ProcedureBuilder builder = new ProcedureBuilder();
            this.SetPlanXml(builder.CreatePlanFromProcedure(prototype));
        }

        /// <summary>
        /// Gets the XML representation of the procedure plan for this procedure type.
        /// </summary>
        public virtual XmlDocument GetPlanXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!string.IsNullOrEmpty(_planXml))
                xmlDoc.LoadXml(_planXml);
            return xmlDoc;
        }

        /// <summary>
        /// Sets the XML representation of the procedure plan for this procedure type.
        /// </summary>
        public virtual void SetPlanXml(XmlDocument value)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb)))
            {
                writer.Formatting = Formatting.Indented;
                value.Save(writer);
                _planXml = sb.ToString();
            }
        }

        /// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}
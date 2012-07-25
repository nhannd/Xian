#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
            :this(id, name, null, null, 0)
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
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

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using ClearCanvas.Enterprise.Hibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class MimeDocumentDataBroker
    {
        private static readonly string TABLE_NAME = "MimeDocumentData_";
        private static readonly string COLUMN_NAME = "BinaryData_";
        private static readonly string COLUMN_TYPE = "varbinary(max) not null";

        /// <summary>
        /// Extension to generate DDL to create and initialize the Accession Sequence table
        /// </summary>
        [ExtensionOf(typeof(DdlScriptGeneratorExtensionPoint))]
        public class MimeDocumentDataDdlScriptGenerator : IDdlScriptGenerator
        {
            #region IDdlScriptGenerator Members

            public string[] GenerateCreateScripts(Configuration config, Dialect dialect)
            {
                string defaultSchema = config.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);
                string tableName = !string.IsNullOrEmpty(defaultSchema) ? defaultSchema + "." + TABLE_NAME : TABLE_NAME;

                return new string[]
				{
                    string.Format("alter table {0} alter column {1} {2}", tableName, COLUMN_NAME, COLUMN_TYPE)
				};
            }

            public string[] GenerateDropScripts(Configuration config, Dialect dialect)
            {
                return new string[] { };    // nothing to do
            }

            #endregion
        }
    }
}

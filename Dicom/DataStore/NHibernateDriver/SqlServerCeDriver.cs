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

using System.Collections;
using System.Data;
using NHibernate.Cfg;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using NHibernate.Driver;

namespace ClearCanvas.Dicom.DataStore.NHibernateDriver
{
    /// <summary>
    /// A NHibernate Driver for using the SqlClient DataProvider
    /// </summary>
    public class SqlServerCeDriver : ReflectionBasedDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlClientDriver"/> class.
        /// </summary>
        public SqlServerCeDriver()
            : base(
            "System.Data.SqlServerCe",
            "System.Data.SqlServerCe.SqlCeConnection",
            "System.Data.SqlServerCe.SqlCeCommand")
        {
        }

        //Only available in NHibernate 1.2
        //public override void Configure(IDictionary settings)
        //{
        //    base.Configure(settings);
        //    prepareSql = PropertiesHelper.GetBoolean(Environment.PrepareSql, settings, false);
        //}

        /// <summary>
        /// MsSql requires the use of a Named Prefix in the SQL statement.  
        /// </summary>
        /// <remarks>
        /// <c>true</c> because MsSql uses "<c>@</c>".
        /// </remarks>
        public override bool UseNamedPrefixInSql
        {
            get { return true; }
        }

        /// <summary>
        /// MsSql requires the use of a Named Prefix in the Parameter.  
        /// </summary>
        /// <remarks>
        /// <c>true</c> because MsSql uses "<c>@</c>".
        /// </remarks>
        public override bool UseNamedPrefixInParameter
        {
            get { return true; }
        }

        /// <summary>
        /// The Named Prefix for parameters.  
        /// </summary>
        /// <value>
        /// Sql Server uses <c>"@"</c>.
        /// </value>
        public override string NamedPrefix
        {
            get { return "@"; }
        }

        /// <summary>
        /// The SqlClient driver does NOT support more than 1 open IDataReader
        /// with only 1 IDbConnection.
        /// </summary>
        /// <value><c>false</c> - it is not supported.</value>
        /// <remarks>
        /// Ms Sql 2000 (and 7) throws an Exception when multiple DataReaders are 
        /// attempted to be Opened.  When Yukon comes out a new Driver will be 
        /// created for Yukon because it is supposed to support it.
        /// </remarks>
        public override bool SupportsMultipleOpenReaders
        {
            get { return false; }
        }

        //Only available in NHibernate 1.2
        //public override IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
        //{
        //    IDbCommand command = base.GenerateCommand(type, sqlString, parameterTypes);
        //    if (prepareSql)
        //    {
        //        SqlClientDriver.SetParameterSizes(command.Parameters, parameterTypes);
        //    }

        //    return command;
        //}
    }
}
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

using System;
using System.Data;

using NHibernate.SqlCommand;
using Environment = NHibernate.Cfg.Environment;
using NHibernate.Dialect;

namespace ClearCanvas.Dicom.DataStore.NHibernateDriver
{
    /// <summary>
    /// A dialect for SQL Server Everywhere.
    /// </summary>
    public class MsSqlCeDialect : Dialect
    {
        public MsSqlCeDialect()
        {
            RegisterColumnType(DbType.AnsiStringFixedLength, "NCHAR(255)");
            RegisterColumnType(DbType.AnsiStringFixedLength, 4000, "NCHAR");
            RegisterColumnType(DbType.AnsiString, "NVARCHAR(255)");
            RegisterColumnType(DbType.AnsiString, 4000, "NVARCHAR");
            RegisterColumnType(DbType.AnsiString, 1073741823, "NTEXT");
            RegisterColumnType(DbType.Binary, "VARBINARY(4000)");
            RegisterColumnType(DbType.Binary, 4000, "VARBINARY($1)");
            RegisterColumnType(DbType.Binary, 1073741823, "IMAGE");
            RegisterColumnType(DbType.Boolean, "BIT");
            RegisterColumnType(DbType.Byte, "TINYINT");
            RegisterColumnType(DbType.Currency, "MONEY");
            RegisterColumnType(DbType.DateTime, "DATETIME");
            RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
            RegisterColumnType(DbType.Decimal, 19, "NUMERIC(19, $1)");
            RegisterColumnType(DbType.Double, "FLOAT");
            RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
            RegisterColumnType(DbType.Int16, "SMALLINT");
            RegisterColumnType(DbType.Int32, "INT");
            RegisterColumnType(DbType.Int64, "BIGINT");
            RegisterColumnType(DbType.Single, "REAL"); //synonym for FLOAT(24) 
            RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
            RegisterColumnType(DbType.StringFixedLength, 4000, "NCHAR($1)");
            RegisterColumnType(DbType.String, "NVARCHAR(255)");
            RegisterColumnType(DbType.String, 4000, "NVARCHAR($1)");
            RegisterColumnType(DbType.String, 1073741823, "NTEXT");
            RegisterColumnType(DbType.Time, "DATETIME");

            DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SqlServerCeDriver";
            DefaultProperties[Environment.PrepareSql] = "false";
        }

        public override string AddColumnString
        {
            get { return "add"; }
        }

        public override string NullColumnString
        {
            get { return " null"; }
        }

        public override bool QualifyIndexName
        {
            get { return false; }
        }

        //Only available in NHibernate 1.2
        //public override bool SupportsMultipleQueries
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        //Only available in NHibernate 1.2
        //public override string ForUpdateString
        //{
        //    get { return string.Empty; }
        //}

        //Only available in NHibernate 1.2
        //public override SqlString AddIdentitySelectToInsert(SqlString insertSql, string identityColumn, string tableName)
        //{
        //    return null;
        //}

        public override bool SupportsIdentityColumns
        {
            get { return true; }
        }

        //Only available in NHibernate 1.2
        //public override string GetIdentitySelectString(string identityColumn, string tableName)
        //{
        //    return "select @@IDENTITY";
        //}

        public override string IdentityColumnString
        {
            get { return "IDENTITY NOT NULL"; }
        }

        public override bool SupportsLimit
        {
            get { return false; }
        }

        public override bool SupportsLimitOffset
        {
            get { return false; }
        }

        public override bool SupportsVariableLimit
        {
            get { return false; }
        }
    }
}

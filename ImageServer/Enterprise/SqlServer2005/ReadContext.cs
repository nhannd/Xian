#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data.SqlClient;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// Provides implementation of <see cref="IReadContext"/> for use with ADO.NET and Sql Server.
    /// </summary>
    public class ReadContext : PersistenceContext,IReadContext,IDisposable
    {
        internal ReadContext(SqlConnection connection, ITransactionNotifier transactionNotifier)
            : base(connection, transactionNotifier)
        { }

        #region IDisposable Members

        /// <summary>
        /// Commits the transaction (does not flush anything to the database)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                    // end the transaction
            }

            // important to call base class to close the session, etc.
            base.Dispose(disposing);
        }

        #endregion
    }
}

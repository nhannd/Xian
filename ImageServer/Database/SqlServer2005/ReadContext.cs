using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public class ReadContext : PersistenceContext,IReadContext
    {
        internal ReadContext(SqlConnection connection)
            : base(connection)
        { }
       
    }
}

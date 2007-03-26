using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public class HqlException : PersistenceException
    {
        public HqlException(string message)
            :base(message, null)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Data.Hibernate.Hql
{
    public abstract class HqlElement
    {
        public abstract string Hql { get; }
    }
}

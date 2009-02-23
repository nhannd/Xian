using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Model
{
    public abstract class ElementInfo
    {
        public abstract bool IsSame(ElementInfo other);
        public abstract bool IsIdentical(ElementInfo other);
        public abstract string SortKey { get; }
    }
}

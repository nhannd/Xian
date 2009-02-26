using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    public abstract class ElementInfo
    {
        public abstract string Identity { get; }

        public override bool Equals(object obj)
        {
            ElementInfo that = obj as ElementInfo;
            if (that == null)
                return false;
            return this.GetType() == that.GetType() && this.Identity == that.Identity;
        }

        public override int GetHashCode()
        {
            return this.Identity.GetHashCode();
        }
    }
}

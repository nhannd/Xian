using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public class HqlFrom : HqlElement
    {
        private string _alias;
        private string _entityClass;

        public HqlFrom(string alias, string entityClass)
        {
            _alias = alias;
            _entityClass = entityClass;
        }

        public override string Hql
        {
            get { return string.Format("{0} {1}", _entityClass, _alias); }
        }
    }
}

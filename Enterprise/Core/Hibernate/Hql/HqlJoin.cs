using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Data.Hibernate.Hql
{
    public class HqlJoin : HqlElement
    {
        private string _alias;
        private string _source;

        public HqlJoin(string alias, string source)
        {
            _alias = alias;
            _source = source;
        }

        public override string Hql
        {
            get { return string.Format("join {0} {1}", _source, _alias); }
        }
    }
}

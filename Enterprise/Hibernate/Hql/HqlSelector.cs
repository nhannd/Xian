using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public class HqlSelector : HqlElement
    {
        private string _property;
        private string _source;

        public HqlSelector(string property, string source)
        {
            _property = property;
            _source = source;
        }

        public override string Hql
        {
            get { return _source; }
        }

        public string Property
        {
            get { return _property; }
        }
    }
}

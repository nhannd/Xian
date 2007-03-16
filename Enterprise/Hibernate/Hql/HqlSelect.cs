using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public class HqlSelect : HqlElement
    {
        private string _variable;

        /// <summary>
        /// Constructs a select on the specified HQL variable
        /// </summary>
        /// <param name="variable">The HQL variable to select</param>
        public HqlSelect(string variable)
        {
            _variable = variable;
        }

        public override string Hql
        {
            get { return _variable; }
        }
    }
}

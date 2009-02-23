using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class DropTableChange : Change
    {
        public DropTableChange(TableInfo table)
			:base(table)
        {

        }

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class AddTableChange : RelationalModelChange
    {
        public AddTableChange(TableInfo table)
			: base(table)
		{
        }

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}

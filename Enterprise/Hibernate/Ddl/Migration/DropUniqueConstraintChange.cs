using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class DropUniqueConstraintChange : RelationalModelChange
    {
    	private readonly ConstraintInfo _constraint;

        public DropUniqueConstraintChange(TableInfo table, ConstraintInfo constraint)
			: base(table)
		{
        	_constraint = constraint;
        }

    	public ConstraintInfo Constraint
    	{
			get { return _constraint; }
    	}

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}

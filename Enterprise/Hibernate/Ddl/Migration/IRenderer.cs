using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	interface IRenderer
	{
		Statement[] Render(AddTableChange change);
		Statement[] Render(DropTableChange change);
		Statement[] Render(AddColumnChange change);
		Statement[] Render(DropColumnChange change);
		Statement[] Render(AddIndexChange change);
		Statement[] Render(DropIndexChange change);
		Statement[] Render(AddForeignKeyChange change);
		Statement[] Render(DropForeignKeyChange change);
		Statement[] Render(AddUniqueConstraintChange change);
		Statement[] Render(DropUniqueConstraintChange change);
		Statement[] Render(ColumnPropertiesChange change);
	}
}
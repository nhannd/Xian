using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	/// <summary>
	/// Defines an interface to an object that renders instances of <see cref="RelationalModelChange"/>
	/// to SQL statements for a given RDBMS/dialect.
	/// </summary>
	interface IRenderer
	{
		/// <summary>
		/// Allows the renderer to add or remove changes from the change set prior to rendering.
		/// This is necessary because, depending on the DDL dialect, some changes may be redundant,
		/// and can be removed, or other changes may need to be added.
		/// </summary>
		/// <param name="changes"></param>
		/// <returns></returns>
        IEnumerable<RelationalModelChange> PreFilter(IEnumerable<RelationalModelChange> changes);

		Statement[] Render(AddTableChange change);
		Statement[] Render(DropTableChange change);
		Statement[] Render(AddColumnChange change);
		Statement[] Render(DropColumnChange change);
		Statement[] Render(AddIndexChange change);
		Statement[] Render(DropIndexChange change);
		Statement[] Render(AddPrimaryKeyChange change);
		Statement[] Render(DropPrimaryKeyChange change);
		Statement[] Render(AddForeignKeyChange change);
		Statement[] Render(DropForeignKeyChange change);
		Statement[] Render(AddUniqueConstraintChange change);
		Statement[] Render(DropUniqueConstraintChange change);
		Statement[] Render(ModifyColumnChange change);
		Statement[] Render(AddEnumValueChange change);
		Statement[] Render(DropEnumValueChange change);
	}
}
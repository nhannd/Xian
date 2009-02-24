using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;
using NHibernate.Dialect;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration.Renderers;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	class Renderer : IRenderer
	{
		/// <summary>
		/// Gets the renderer implementation that corresponds to the specified dialect.
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public static IRenderer GetRenderer(Dialect dialect)
		{
			// TODO use dialect to choose renderer
			return new MsSqlRenderer(dialect);
		}

		private readonly Dialect _dialect;

		protected Renderer(Dialect dialect)
		{
			_dialect = dialect;
		}

		#region IRenderer Members

		public virtual Statement[] Render(AddTableChange change)
		{
			TableInfo table = change.Table;

			StringBuilder buf = new StringBuilder("create table ")
				.Append(GetQualifiedName(table))
				.Append(" (");

			string columns = StringUtilities.Combine(
				CollectionUtils.Map<ColumnInfo, string>(table.Columns,
					delegate(ColumnInfo col)
					{
						return GetColumnDefinitionString(col);
					}), ", ");

			buf.Append(columns);

			if (table.PrimaryKey != null)
			{
				buf.Append(',').Append(GetPrimaryKeyString(table.PrimaryKey));
			}

			foreach (ConstraintInfo uk in table.UniqueKeys)
			{
				buf.Append(',').Append(GetUniqueConstraintString(uk));
			}

			buf.Append(")");

			return new Statement[] { new Statement(buf.ToString()) };
		}

		public virtual Statement[] Render(DropTableChange change)
		{
			return new Statement[] { new Statement(_dialect.GetDropTableString(GetQualifiedName(change.Table))), };
		}

		public virtual Statement[] Render(AddColumnChange change)
		{
			string sql = string.Format("alter table {0} add {1}", GetQualifiedName(change.Table), GetColumnDefinitionString(change.Column));
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(DropColumnChange change)
		{
			string sql = string.Format("alter table {0} drop column {1}", GetQualifiedName(change.Table), change.Column.Name);
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(AddIndexChange change)
		{
			IndexInfo index = change.Index;
			StringBuilder buf = new StringBuilder("create index ")
				.Append(_dialect.QualifyIndexName ? index.Name : NHibernate.Util.StringHelper.Unqualify(index.Name))
				.Append(" on ")
				.Append(GetQualifiedName(change.Table))
				.Append(" (");

			buf.Append(StringUtilities.Combine(index.Columns, ", "));
			buf.Append(")");

			return new Statement[] { new Statement(buf.ToString()) };
		}

		public virtual Statement[] Render(DropIndexChange change)
		{
			string sql = string.Format("drop index {0}.{1}", GetQualifiedName(change.Table), change.Index.Name);
			return new Statement[] { new Statement(sql) };
		}

		public Statement[] Render(AddPrimaryKeyChange change)
		{
			string sql = string.Format("alter table {0} add constraint {1} {2}",
				GetQualifiedName(change.Table),
				change.PrimaryKey.Name,
				GetPrimaryKeyString(change.PrimaryKey));

			return new Statement[] { new Statement(sql) };
		}

		public Statement[] Render(DropPrimaryKeyChange change)
		{
			string sql = "alter table " + GetQualifiedName(change.Table) + _dialect.GetDropIndexConstraintString(change.PrimaryKey.Name);
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(AddForeignKeyChange change)
		{
			ForeignKeyInfo fk = change.ForeignKey;
			string[] cols = fk.Columns.ToArray();
			string[] refcols = fk.ReferencedColumns.ToArray();

			string sql = _dialect.GetAddForeignKeyConstraintString(fk.Name, cols, GetQualifiedName(change.Table.Schema, fk.ReferencedTable), refcols);
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(DropForeignKeyChange change)
		{
			string sql = string.Format("alter table {0} {1}", GetQualifiedName(change.Table),
							  _dialect.GetDropForeignKeyConstraintString(change.ForeignKey.Name));

			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(AddUniqueConstraintChange change)
		{
			string sql = string.Format("alter table {0} add constraint {1} {2}",
				GetQualifiedName(change.Table),
				change.Constraint.Name,
				GetUniqueConstraintString(change.Constraint));

			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(DropUniqueConstraintChange change)
		{
			string sql = "alter table " + GetQualifiedName(change.Table) + _dialect.GetDropIndexConstraintString(change.Constraint.Name);
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(ModifyColumnChange change)
		{
			string sql = string.Format("alter table {0} alter column {1}", GetQualifiedName(change.Table), GetColumnDefinitionString(change.Column));
			return new Statement[] { new Statement(sql) };
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Gets the schema qualified name of the Table.
		/// </summary>
		protected string GetQualifiedName(TableInfo table)
		{
			return GetQualifiedName(table.Schema, table.Name);
		}

		protected string GetQualifiedName(string schema, string table)
		{
			return schema == null ? table : schema + "." + table;
		}

		protected string GetPrimaryKeyString(ConstraintInfo pk)
		{
			return string.Format(" primary key ({0})", StringUtilities.Combine(pk.Columns, ", "));
		}

		protected string GetUniqueConstraintString(ConstraintInfo uk)
		{
			return string.Format(" unique ({0})", StringUtilities.Combine(uk.Columns, ", "));
		}

		protected string GetColumnDefinitionString(ColumnInfo col)
		{
			StringBuilder colStr = new StringBuilder();
			colStr.Append(col.Name).Append(' ').Append(col.SqlType);

			if (col.Nullable)
			{
				colStr.Append(_dialect.NullColumnString);
			}
			else
			{
				colStr.Append(" not null");
			}

			if (col.Unique)
			{
				if (_dialect.SupportsUnique)
				{
					colStr.Append(" unique");
				}
				else
				{
					// TODO: create an independent unique constraint instead
					throw new NotSupportedException("Dialect does not support unique columns.");
				}
			}
			return colStr.ToString();
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Metadata;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;
using System.Collections;
using NHibernate.Type;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    class EnumForeignKeyConstraintGenerator : DdlScriptGenerator
    {
        class FKConstraint
        {
            public Table ConstrainedTable;
            public Column ConstrainedColumn;
            public Table ReferencedTable;

            public string GetCreateScript(Dialect dialect, string defaultSchema)
            {
                // don't really know if this will be unique or not...
                int unique = ConstrainedTable.Name.GetHashCode() ^ ConstrainedColumn.Name.GetHashCode() ^ ReferencedTable.Name.GetHashCode();
                string fkName = string.Format("FK{0}", unique.ToString("X"));

                return string.Format("alter table {0} add constraint {1} foreign key ({2}) references {3}",
                    this.ConstrainedTable.GetQualifiedName(dialect, defaultSchema),
                    fkName,
                    this.ConstrainedColumn.GetQuotedName(dialect),
                    this.ReferencedTable.GetQualifiedName(dialect, defaultSchema));
            }
        }

        public override string[] GenerateCreateScripts(PersistentStore store, Dialect dialect)
        {
            string defaultSchema = store.Configuration.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);

            List<FKConstraint> constraints = new List<FKConstraint>();
            foreach (PersistentClass pc in store.Configuration.ClassMappings)
            {
                CreateConstraints(store, pc.PropertyCollection, constraints);
            }

            return CollectionUtils.Map<FKConstraint, string, List<string>>(constraints,
                delegate(FKConstraint c) { return c.GetCreateScript(dialect, defaultSchema); }).ToArray();
        }

        public override string[] GenerateDropScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            return new string[0];
        }

        private void CreateConstraints(PersistentStore store, ICollection properties, List<FKConstraint> constraints)
        {
            foreach (Property prop in properties)
            {
                if (prop.Value is Component)
                {
                    // recur on component properties
                    Component comp = prop.Value as Component;
                    CreateConstraints(store, comp.PropertyCollection, constraints);
                }
                else if (prop.Value is Collection)
                {
                    // recur on collections-of-values (composite-element)
                    Collection coll = prop.Value as Collection;
                    if (coll.Element is Component)
                    {
                        Component comp = coll.Element as Component;
                        CreateConstraints(store, comp.PropertyCollection, constraints);
                    }
                }
                else
                {
                    // is this property mapped with an EnumHbm class???
                    if (prop.Type is EnumStringType)
                    {
                        Type enumClass = GetEnumValueClassForEnumType(prop.Type.ReturnedClass);

                        // build a constraint for this column
                        FKConstraint constraint = new FKConstraint();
                        constraint.ConstrainedTable = prop.Value.Table;
                        constraint.ConstrainedColumn = CollectionUtils.FirstElement<Column>(prop.ColumnCollection);
                        constraint.ReferencedTable = GetTableForEnumClass(enumClass, store);

                        constraints.Add(constraint);
                    }
                }
            }
        }

        private Type GetEnumValueClassForEnumType(Type enumType)
        {
            EnumValueClassAttribute attr = CollectionUtils.FirstElement<EnumValueClassAttribute>(
                enumType.GetCustomAttributes(typeof(EnumValueClassAttribute), false));

            if (attr == null)
                throw new Exception(string.Format("{0} is not marked with the EnumValueClassAttribute", enumType.FullName));

            return attr.EnumValueClass;
        }

        private Table GetTableForEnumClass(Type enumClass, PersistentStore store)
        {
            PersistentClass pclass = CollectionUtils.SelectFirst<PersistentClass>(store.Configuration.ClassMappings,
                delegate(PersistentClass c) { return c.MappedClass == enumClass; });

            if (pclass == null)
                throw new Exception(string.Format("{0} is not a persistent class", enumClass.FullName));

            return pclass.Table;
        }

        #region unused

        private void Write(string text, int depth)
        {
            string tabs = "";
            for (int i = 0; i < depth; i++) tabs += "\t";
            Console.WriteLine(tabs + text);
        }

        private void WriteProperties(IEnumerable properties, int depth)
        {
            foreach (Property prop in properties)
            {
                if (prop.Value is Component)
                {
                    Write(prop.Name, depth);
                    Component comp = prop.Value as Component;
                    WriteProperties(comp.PropertyCollection, depth + 1);
                }
                else if (prop.Value is Collection)
                {
                    Write(prop.Name, depth);
                    Collection coll = prop.Value as Collection;
                    if (coll.Element is Component)
                    {
                        Component comp = coll.Element as Component;
                        WriteProperties(comp.PropertyCollection, depth + 1);
                    }
                }
                else
                {
                    if (prop.Type is EnumStringType)
                    {
                        Write(prop.Name, depth);
                        foreach (Column col in prop.ColumnCollection)
                        {
                            Write(prop.Value.Table.Name + "." + col.Name + ": " + prop.Type.ReturnedClass.FullName, depth + 1);
                        }
                    }
                    else
                    {
                        Write(prop.Name, depth);
                    }
                }
            }
        }
        #endregion
    }
}

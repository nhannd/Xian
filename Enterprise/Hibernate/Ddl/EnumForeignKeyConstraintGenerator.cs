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

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    class EnumForeignKeyConstraintGenerator : DdlScriptGenerator
    {
        class FKConstraint
        {
            public string ConstrainedTable;
            public string ConstrainedColumn;
            public string ReferencedTable;

            public string GetCreateScript()
            {
                // don't really know if this will be unique or not...
                int unique = ConstrainedTable.GetHashCode() ^ ConstrainedColumn.GetHashCode() ^ ReferencedTable.GetHashCode();

                return string.Format("alter table {0} add constraint {1} foreign key ({2}) references {3}",
                    this.ConstrainedTable,
                    string.Format("FK{0}", unique.ToString("X")),
                    this.ConstrainedColumn,
                    this.ReferencedTable);
            }
        }

        public override string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            List<FKConstraint> constraints = new List<FKConstraint>();
            foreach (PersistentClass pc in store.Configuration.ClassMappings)
            {
                CreateConstraints(store, pc.PropertyCollection, constraints);
            }

            return CollectionUtils.Map<FKConstraint, string, List<string>>(constraints,
                delegate(FKConstraint c) { return c.GetCreateScript(); }).ToArray();
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
                        constraint.ConstrainedTable = prop.Value.Table.Name;
                        constraint.ConstrainedColumn = CollectionUtils.FirstElement<Column>(prop.ColumnCollection).Name;
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

        private string GetTableForEnumClass(Type enumClass, PersistentStore store)
        {
            PersistentClass pclass = CollectionUtils.SelectFirst<PersistentClass>(store.Configuration.ClassMappings,
                delegate(PersistentClass c) { return c.MappedClass == enumClass; });

            if (pclass == null)
                throw new Exception(string.Format("{0} is not a persistent class", enumClass.FullName));

            return pclass.Table.Name;
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

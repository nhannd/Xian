using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;
using NHibernate.Mapping;
using Iesi.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using NHibernate.Type;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Adds DB indexes on foreign key columns to the Hibernate relational model, based on a set of rules described below.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Rules:
    /// 1. For entities, create indexes on all references to other entities and enums.
    /// 2. For collections of values, create indexes only on the reference to the owner.
    /// 3. For many-to-many collection tables, create 2 indexes, each containing both of the columns, but varying which column is
    /// listed first.  This should improve join performance, going in either direction, by allowing the DB to bypass the join table
    /// altogether, using only the index.  This is explained to some degree here http://msdn2.microsoft.com/en-us/library/ms191195.aspx.
    /// </para>
    /// <para>
    /// This class make decisions about which indexes to create based on foreign keys.  Therefore, 
    /// ensure the that <see cref="EnumForeignKeyProcessor"/> and any other processors
    /// that create foreign keys are run prior to this processor.
    /// </para>
    /// </remarks>
    class ForeignKeyIndexProcessor : IndexCreatorBase
    {
        #region Overrides

        public override void Process(Configuration config)
        {
            // Rules:
            // 1. For entities, create indexes on all references to other entities and enums
            foreach (PersistentClass pc in config.ClassMappings)
            {
                CreateIndexes(config, pc.PropertyCollection);
            }

            // 2. For collections of values, create indexes only on the reference to the owner
            // 3. For many-to-many collection tables, create indexes on both columns together, going in both directions??

            foreach (Collection collection in config.CollectionMappings)
            {
                CreateIndexes(config, collection);
            }
        }

        #endregion

		private void CreateIndexes(Configuration config, Collection collection)
        {
            if(collection.Element is ManyToOne)
            {
                // many-to-many collection

                // collect all columns that participate in foreign keys
                HybridSet columns = new HybridSet();
                foreach (ForeignKey fk in collection.CollectionTable.ForeignKeyCollection)
                {
                    columns.AddAll(fk.ColumnCollection);
                }

                // there should always be exactly 2 "foreign key' columns in a many-many join table, AFAIK
                if (columns.Count != 2)
                {
                    throw new Exception("SNAFU");
                }

                List<Column> indexColumns = new List<Column>(new TypeSafeEnumerableWrapper<Column>(columns));

                // create two indexes, each containing both columns, going in both directions
                CreateIndex(collection.CollectionTable, indexColumns);

                indexColumns.Reverse();
                CreateIndex(collection.CollectionTable, indexColumns);
            }
            else
            {
                // this is a value collection, or a one-to-many collection

                // find the foreign-key that refers back to the owner table (assume there is only one of these - is this always true??)
                ForeignKey foreignKey = CollectionUtils.SelectFirst<ForeignKey>(collection.CollectionTable.ForeignKeyCollection,
                    delegate (ForeignKey fk) { return Equals(fk.ReferencedTable, collection.Table); });

                // create an index on all columns in this foreign key
                if(foreignKey != null)
                {
                    CreateIndex(collection.CollectionTable, new TypeSafeEnumerableWrapper<Column>(foreignKey.ColumnCollection));
                }
            }
        }

		private void CreateIndexes(Configuration config, ICollection properties)
        {
            foreach (Property prop in properties)
            {
                if (prop.Value is Component)
                {
                    // recur on component properties
                    Component comp = (Component) prop.Value;
                    CreateIndexes(config, comp.PropertyCollection);
                }
                else
                {
                    // is this property mapped with an EnumHbm class, or is it a many-to-one??
                    if (prop.Type is EnumStringType || prop.Type is ManyToOneType)
                    {
                        // index this column
                        Table indexedTable = prop.Value.Table;
                        Column indexedColumn = CollectionUtils.FirstElement<Column>(prop.ColumnCollection);
                        CreateIndex(indexedTable, new Column[] { indexedColumn });
                    }
                }
            }
        }
    }
}

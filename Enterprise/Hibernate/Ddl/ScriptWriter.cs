using System;
using System.IO;
using System.Collections.Generic;

using NHibernate.Cfg;
using ClearCanvas.Common;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Defines an extension point for contributing DDL script generators to the database definition.
    /// Note that extensions of this point must not have dependencies on the existence of other database objects,
    /// as the order in which each extension is processed is not deterministic.
    /// </summary>
    [ExtensionPoint]
    public class DdlScriptGeneratorExtensionPoint : ExtensionPoint<IDdlScriptGenerator>
    {
    }

    /// <summary>
    /// Utility class that generates a database creation and/or drop script and writes the script to a <see cref="StreamWriter"/>.
    /// </summary>
    public class ScriptWriter
    {
        private List<IDdlScriptGenerator> _generators;
        private PersistentStore _store;
        private Dialect _dialect;

        public ScriptWriter(PersistentStore store, Dialect dialect)
        {
            _store = store;
            _dialect = dialect;

            _generators = new List<IDdlScriptGenerator>();

            // the order of generator execution is important, so add the static generators first
            _generators.Add(new TableSchemaGenerator());
            _generators.Add(new EnumForeignKeyConstraintGenerator());
            _generators.Add(new EnumValueInsertGenerator());

            // subsequently we can add extension generators, with uncontrolled ordering
            foreach (IDdlScriptGenerator generator in (new DdlScriptGeneratorExtensionPoint().CreateExtensions()))
            {
                _generators.Add(generator);
            }
        }

        /// <summary>
        /// Writes a database creation script to the specified <see cref="StreamWriter"/>
        /// </summary>
        /// <param name="sw"></param>
        public void WriteCreateScript(StreamWriter sw)
        {
            foreach (IDdlScriptGenerator gen in _generators)
            {
                foreach (string script in gen.GenerateCreateScripts(_store, _dialect))
                {
                    sw.WriteLine(script);
                }
            }
        }

        /// <summary>
        /// Writes a database drop script to the specified <see cref="StreamWriter"/>
        /// </summary>
        /// <param name="sw"></param>
        public void WriteDropScript(StreamWriter sw)
        {
            foreach (IDdlScriptGenerator gen in _generators)
            {
                foreach (string script in gen.GenerateDropScripts(_store, _dialect))
                {
                    sw.WriteLine(script);
                }
            }
        }
    }
}

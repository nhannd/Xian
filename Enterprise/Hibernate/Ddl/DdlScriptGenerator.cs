using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using NHibernate.Metadata;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    public abstract class DdlScriptGenerator : IDdlScriptGenerator
    {
        #region IDdlScriptGenerator Members

        public abstract string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect);

        public abstract string[] GenerateDropScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect);

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database
{
    public class ProcedureEntityRef : EntityRef
    {
        public ProcedureEntityRef(Type entityClass, object entityOid, int version)
            : base(entityClass, entityOid, version)
        {
        }
    }
}

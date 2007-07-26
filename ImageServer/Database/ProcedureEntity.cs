using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database
{
    public abstract class ProcedureEntity : Entity
    {
        public ProcedureEntity()
            : base()
        {

        }
        
        public void SetOid(object oid)
        {
            OID = oid;
        }
    }
}

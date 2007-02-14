using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Enterprise.Support
{
    public abstract class EntityInfo : DomainObjectInfo
    {
        public abstract EntityRefBase GetEntityRef();
    }
}

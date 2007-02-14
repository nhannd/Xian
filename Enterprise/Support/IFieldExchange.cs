using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Support
{
    public delegate void SetFieldValueDelegate(object obj, object value);
    public delegate object GetFieldValueDelegate(object obj);

    public interface IFieldExchange
    {
        void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx);
        void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx);
    }
}

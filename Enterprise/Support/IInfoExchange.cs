using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Support
{
    public interface IInfoExchange
    {
        object GetInfoFromObject(object pobj, IPersistenceContext pctx);
        object GetObjectFromInfo(object info, IPersistenceContext pctx);
    }
}

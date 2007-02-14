using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Support
{
    public class PrimitiveTypeInfoExchange : IInfoExchange
    {
        #region IConversion Members

        public object GetInfoFromObject(object pobj, IPersistenceContext pctx)
        {
            return pobj;
        }

        public object GetObjectFromInfo(object info, IPersistenceContext pctx)
        {
            return info;
        }

        #endregion
    }
}

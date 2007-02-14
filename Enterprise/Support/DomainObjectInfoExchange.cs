using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Enterprise.Support
{
    public class DomainObjectInfoExchange<TDomainObject, TDomainObjectInfo> : IInfoExchange
        where TDomainObject : DomainObject, new()
        where TDomainObjectInfo : DomainObjectInfo, new()
    {
        private List<IFieldExchange> _fieldExchangers = new List<IFieldExchange>();

        public DomainObjectInfoExchange()
        {
            // validation
            if(!typeof(TDomainObject).Equals(DomainObjectExchangeBuilder.GetAssociatedDomainClass(typeof(TDomainObjectInfo))))
                throw new Exception("Cannot convert between these types");

            // build the conversion
            _fieldExchangers.AddRange(
                DomainObjectExchangeBuilder.CreateFieldExchangers(typeof(TDomainObject), typeof(TDomainObjectInfo)));
        }

        protected IList<IFieldExchange> FieldExchangers
        {
            get { return _fieldExchangers; }
        }

        public TDomainObjectInfo GetInfoFromObject(TDomainObject obj, IPersistenceContext pctx)
        {
            if (obj == null) return null;
            TDomainObjectInfo info = new TDomainObjectInfo();
            foreach (IFieldExchange fe in _fieldExchangers)
            {
                fe.SetInfoFieldFromObject(obj, info, pctx);
            }
            return info;
        }

        public TDomainObject GetObjectFromInfo(TDomainObjectInfo info, IPersistenceContext pctx)
        {
            if (info == null) return null;
            TDomainObject obj = new TDomainObject();
            foreach (IFieldExchange fe in _fieldExchangers)
            {
                fe.SetObjectFieldFromInfo(obj, info, pctx);
            }
            return obj;
        }

        #region IConversion Members

        object IInfoExchange.GetInfoFromObject(object pobj, IPersistenceContext pctx)
        {
            return GetInfoFromObject((TDomainObject)pobj, pctx);
        }

        object IInfoExchange.GetObjectFromInfo(object info, IPersistenceContext pctx)
        {
            return GetObjectFromInfo((TDomainObjectInfo)info, pctx);
        }

        #endregion
    }

}

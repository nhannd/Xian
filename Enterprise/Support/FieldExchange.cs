using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;
using System.Collections;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Support
{
    public abstract class FieldExchange : IFieldExchange
    {
        private GetFieldValueDelegate _classFieldGetter;
        private SetFieldValueDelegate _classFieldSetter;
        private GetFieldValueDelegate _infoFieldGetter;
        private SetFieldValueDelegate _infoFieldSetter;

        public FieldExchange(
            GetFieldValueDelegate classFieldGetter,
            SetFieldValueDelegate classFieldSetter,
            GetFieldValueDelegate infoFieldGetter,
            SetFieldValueDelegate infoFieldSetter)
        {
            _classFieldGetter = classFieldGetter;
            _classFieldSetter = classFieldSetter;
            _infoFieldGetter = infoFieldGetter;
            _infoFieldSetter = infoFieldSetter;
        }

        public abstract void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx);
        public abstract void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx);

        protected object GetInfoFieldValue(DomainObjectInfo info)
        {
            return _infoFieldGetter(info);
        }

        protected object GetClassFieldValue(DomainObject pobj)
        {
            return _classFieldGetter(pobj);
        }

        protected void SetInfoFieldValue(DomainObjectInfo info, object value)
        {
            _infoFieldSetter(info, value);
        }

        protected void SetClassFieldValue(DomainObject pobj, object value)
        {
            _classFieldSetter(pobj, value);
        }
    }
}

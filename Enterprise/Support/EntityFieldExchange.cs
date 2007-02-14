using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Support
{
    public class EntityFieldExchange : FieldExchange
    {
        private IInfoExchange _entityConversion;

        public EntityFieldExchange(
            GetFieldValueDelegate classFieldGetter,
            SetFieldValueDelegate classFieldSetter,
            GetFieldValueDelegate infoFieldGetter,
            SetFieldValueDelegate infoFieldSetter,
            IInfoExchange entityConversion)
            : base(classFieldGetter, classFieldSetter, infoFieldGetter, infoFieldSetter)
        {
            _entityConversion = entityConversion;
        }

        public override void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            Entity entity = (Entity)GetClassFieldValue(pobj);
            if (entity != null && pctx.IsProxyLoaded(entity))
            {
                SetInfoFieldValue(info, _entityConversion.GetInfoFromObject(entity, pctx));
            }
        }

        public override void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            EntityInfo entityInfo = (EntityInfo)GetInfoFieldValue(info);
            if (entityInfo != null && entityInfo.GetEntityRef() != null)
            {
                // don't copy any information from the referenced EntityInfo
                // only take its reference
                Entity entity = pctx.Load(entityInfo.GetEntityRef(), EntityLoadFlags.Proxy);    // proxy, no version check!
                SetClassFieldValue(pobj, entity);
            }
        }
    }

}

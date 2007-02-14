using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Support
{
    public class CollectionFieldExchange<TInfoElement> : FieldExchange
    {
        private IInfoExchange _elementConversion;

        public CollectionFieldExchange(
            GetFieldValueDelegate classFieldGetter,
            SetFieldValueDelegate classFieldSetter,
            GetFieldValueDelegate infoFieldGetter,
            SetFieldValueDelegate infoFieldSetter,
            IInfoExchange elementConversion)
            : base(classFieldGetter, classFieldSetter, infoFieldGetter, infoFieldSetter)
        {
            _elementConversion = elementConversion;
        }

        public override void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            IEnumerable pobjCollection = (IEnumerable)GetClassFieldValue(pobj);
            if (pobjCollection != null && pctx.IsCollectionLoaded(pobjCollection))
            {
                List<TInfoElement> infoCollection = new List<TInfoElement>();
                foreach (object element in pobjCollection)
                {
                    infoCollection.Add((TInfoElement)_elementConversion.GetInfoFromObject(element, pctx));
                }
                SetInfoFieldValue(info, infoCollection);
            }
        }

        public override void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            IList infoCollection = (IList)GetInfoFieldValue(info);
            if (infoCollection != null)
            {
                IEnumerable pobjCollection = (IEnumerable)GetClassFieldValue(pobj);
                foreach (object element in infoCollection)
                {
                    if (pobjCollection is IList)
                    {
                        (pobjCollection as IList).Add(_elementConversion.GetObjectFromInfo(element, pctx));
                    }
                    else if (pobjCollection is ISet)
                    {
                        (pobjCollection as ISet).Add(_elementConversion.GetObjectFromInfo(element, pctx));
                    }
                }
            }
        }
    }
}

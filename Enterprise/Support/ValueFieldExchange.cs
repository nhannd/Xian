#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Support
{
    public class ValueFieldExchange : FieldExchange
    {
        private IInfoExchange _valueConversion;

        public ValueFieldExchange(
            GetFieldValueDelegate classFieldGetter,
            SetFieldValueDelegate classFieldSetter,
            GetFieldValueDelegate infoFieldGetter,
            SetFieldValueDelegate infoFieldSetter,
            IInfoExchange valueConversion)
            : base(classFieldGetter, classFieldSetter, infoFieldGetter, infoFieldSetter)
        {
            _valueConversion = valueConversion;
        }

        public override void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            SetInfoFieldValue(info, _valueConversion.GetInfoFromObject(GetClassFieldValue(pobj), pctx));
        }

        public override void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            SetClassFieldValue(pobj, _valueConversion.GetObjectFromInfo(GetInfoFieldValue(info), pctx));
        }
    }
}

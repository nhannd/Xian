#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
    public class EntityRefUtils
    {
        public static int GetVersion(EntityRef entityRef)
        {
            return entityRef.Version;
        }

        public static object GetOID(EntityRef entityRef)
        {
            return entityRef.OID;
        }

        public static Type GetClass(EntityRef entityRef)
        {
            return Type.GetType(entityRef.ClassName, true);
        }

        public static string GetClassName(EntityRef entityRef)
        {
            return entityRef.ClassName;
        }
    }
}

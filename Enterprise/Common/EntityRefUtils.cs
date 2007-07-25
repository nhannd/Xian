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

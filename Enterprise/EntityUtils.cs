using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public static class EntityUtils
    {
        public static bool CheckVersion(EntityRefBase entityRef, Entity entity)
        {
            return entity.Version == entityRef.Version;
        }

        public static int GetVersion(EntityRefBase entityRef)
        {
            return entityRef.Version;
        }

        public static int GetVersion(Entity entity)
        {
            return entity.Version;
        }

        public static long GetOID(EntityRefBase entityRef)
        {
            return entityRef.EntityOID;
        }

        public static Type GetType(EntityRefBase entityRef)
        {
            return entityRef.EntityClass;
        }
    }
}

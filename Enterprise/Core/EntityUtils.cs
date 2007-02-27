using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Provides a set of utility methods for use with instance of <see cref="Entity"/> and <see cref="EntityRef"/>.
    /// This class is not intended for use by application code.
    /// </summary>
    public static class EntityUtils
    {
        public static bool CheckVersion(EntityRefBase entityRef, Entity entity)
        {
            return entity.Version.Equals(entityRef.Version);
        }

        public static int GetVersion(EntityRefBase entityRef)
        {
            return entityRef.Version;
        }

        public static int GetVersion(Entity entity)
        {
            return entity.Version;
        }

        public static object GetOID(EntityRefBase entityRef)
        {
            return entityRef.EntityOID;
        }

        public static Type GetType(EntityRefBase entityRef)
        {
            return Type.GetType(entityRef.EntityClass);
        }
    }
}

using System;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Core
{
	public static class EqualityUtils
	{
		public static bool AreEqual(object x, object y)
		{
			return (x is Entity || y is Entity) ? AreEqual((Entity) x, (Entity) y) : Equals(x, y);
		}

		/// <summary>
		/// This method guarantees efficient handling of uninitialized entity proxies.  If either x or y is a proxy,
		/// the comparison will be performed without causing initialization.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool AreEqual(Entity x, Entity y)
		{
			// if these are the same instance, or both null, they are obviously equal
			if (ReferenceEquals(x, y))
				return true;

			// since they are not both null, they cannot be equal if either is null
			if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;

			// we already know they are not the same instance,
			// but they could still be equal if one is a proxy to the other
			// in this case, they would have to have the same OID
			// and OID is the only property we can test without causing the proxy to load

			// however, if either lacks an OID (eg is transient), we cannot consider them to be equal
			// (even if both have OID == null, there is no basis for saying that they are the same entity)
			if (ReferenceEquals(x.OID, null) || ReferenceEquals(y.OID, null))
				return false;

			// at this point we know that both instance have an OID, so we can just compare
			return x.OID.Equals(y.OID);
		}
	}
}

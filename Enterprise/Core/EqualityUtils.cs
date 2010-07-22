using System;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Utility class that attempts to provide efficient equality checking for any type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class EqualityUtils<T>
	{
		#region EntityEqualityComparer class

		/// <summary>
		/// Implementation of EqualityComparer that provides efficient comparison of entities.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		class EntityEqualityComparer<TEntity> : EqualityComparer<TEntity>
			where TEntity : Entity
		{
			public override bool Equals(TEntity x, TEntity y)
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

			public override int GetHashCode(TEntity obj)
			{
				// use default implementation
				return obj.GetHashCode();
			}
		}

		#endregion


		private static readonly EqualityComparer<T> _comparer;

		/// <summary>
		/// Static initializer.
		/// </summary>
		static EqualityUtils()
		{
			// if T is Entity or a subclass of Entity, then we want to use the special EntityEqualityComparer
			if (typeof(Entity).IsAssignableFrom(typeof(T)))
			{
				var type = typeof (EntityEqualityComparer<>).MakeGenericType(typeof(T), typeof(T));
				_comparer = (EqualityComparer<T>)Activator.CreateInstance(type);
			}
			else
			{
				// otherwise use the .Net frameworks default comparer for the type
				// this comparer seems to be fairly efficient in that it avoids boxing/unboxing where possible
				_comparer = EqualityComparer<T>.Default;
			}
		}


		/// <summary>
		/// Tests if the two specified instances are equal, according to the semantics of the type.
		/// </summary>
		/// <remarks>
		/// This method guarantees efficient handling of uninitialized entity proxies.  If either x or y is a proxy,
		/// the comparison will be performed without causing initialization.
		/// </remarks>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool AreEqual(T x, T y)
		{
			return _comparer.Equals(x, y);
		}
	}
}

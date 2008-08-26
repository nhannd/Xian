using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
	public interface IVersionedEquatable
	{
		bool Equals(object other, bool ignoreVersion);
	}


	/// <summary>
	/// Extends the <see cref="IEquatable{T}"/> interface with an overload of equals that supports
	/// version insensitive equatability.
	/// </summary>
	/// <remarks>
	/// The default implementation of <see cref="IEquatable{T}.Equals(T)"/> should be implemented
	/// such that it is version-sensitive.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public interface IVersionedEquatable<T> : IEquatable<T>, IVersionedEquatable
	{
		/// <summary>
		/// Determine whether two objects are equal, optionally ignoring the version.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="ignoreVersion"></param>
		/// <returns></returns>
		bool Equals(T other, bool ignoreVersion);
	}
}

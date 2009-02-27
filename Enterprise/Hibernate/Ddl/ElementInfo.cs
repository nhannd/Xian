using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines an element in a relational database model.
	/// </summary>
    public abstract class ElementInfo
    {
		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
        public abstract string Identity { get; }

		/// <summary>
		/// Compares elements by identity.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
        public override bool Equals(object obj)
        {
            ElementInfo that = obj as ElementInfo;
            if (that == null)
                return false;
            return this.GetType() == that.GetType() && this.Identity == that.Identity;
        }

		/// <summary>
		/// Gets a hash code based on element identity.
		/// </summary>
		/// <returns></returns>
        public override int GetHashCode()
        {
            return this.Identity.GetHashCode();
        }
    }
}

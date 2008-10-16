using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Defines an interface to an object that is formattable for auditing purposes.
	/// </summary>
	/// <remarks>
	/// This interface is intended to be implemented by classes that are part of the domain model and
	/// wish to specify custom formatting for auditing purposes. Note that entities should not implement
	/// this interface because entities are not audited as objects.  Rather, a change-set captures a set of changes
	/// to individual properties of entities.  Hence, it is the classes that are used as the properties
	/// of entities (typically but not necessarily subclasses of <see cref="ValueObject"/>) that should 
	/// implement this interface.
	/// </remarks>
	public interface IAuditFormattable
	{
		/// <summary>
		/// Asks the implementor to write itself to the specified object writer.
		/// </summary>
		/// <param name="writer"></param>
		void Write(IObjectWriter writer);
	}
}

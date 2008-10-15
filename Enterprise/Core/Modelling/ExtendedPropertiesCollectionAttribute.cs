using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// When applied to a property of a domain object class, indicates that the property
	/// represents a collection of extended properties.
	/// </summary>
	/// <remarks>
	/// This attribute only makes sense when applied to properties of type IDictionary{string,TValue}.
	/// In other words, the property must be a map of strings to some type of value.  The keys are
	/// considered to be extended "properties" of the object that owns the collection.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class ExtendedPropertiesCollectionAttribute : Attribute
	{
	}
}

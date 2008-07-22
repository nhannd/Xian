using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// When applied to an entity class, specifies the name of a boolean property on the class
	/// that acts as a flag to indicate that the entity instance is "de-activated".
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DeactivationFlagAttribute : Attribute
	{
		private readonly string _propertyName;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName"></param>
		public DeactivationFlagAttribute(string propertyName)
		{
			_propertyName = propertyName;
		}

		/// <summary>
		/// Gets the name of a property on the class that acts as a de-activation flag.
		/// </summary>
		public string PropertyName
		{
			get { return _propertyName; }
		}
	}
}
